using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Generators;
using CaptureParse;
using Population.Bucketers;
using Population;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up HttpClient.
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(1);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Load all response tokenizers.
            List<IResponseTokenizer> responseTokenizers = new List<IResponseTokenizer>();
            responseTokenizers.Add(new JsonTokenizer());
            responseTokenizers.Add(new BearerTokenizer());
            responseTokenizers.Add(new HtmlFormTokenizer());

            // Load all request tokenizers.
            List<IRequestTokenizer> requestTokenizers = new List<IRequestTokenizer>();
            requestTokenizers.Add(new JsonTokenizer());
            requestTokenizers.Add(new QueryTokenizer());
            requestTokenizers.Add(new BearerTokenizer());
            requestTokenizers.Add(new KnownUrlArgumentTokenizer());
            requestTokenizers.Add(new HtmlFormTokenizer());
            //requestTokenizers.Add(new CookieTokenizer());

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("username", "asdfg@asdfg.com", "", Types.String));
            startingData.Add(new JsonToken("password", "asdfg", "", Types.String));

            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\web_store.txt", 3));
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\xss_payloads_many.txt", 10));
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\blns.txt", 10));

            PopulationManager population = new PopulationManager();
            foreach (RequestResponsePair endpoint in InitializeEndpoints())
            {
                endpoint.Tokenize(requestTokenizers, responseTokenizers);
                population.AddEndpoint(endpoint, new TokenNameBucketer());
            }

            // Start with an initial population of one empty request sequence.
            //population.AddResponse(new RequestSequence(), new TokenCollection());
            
            // In each generation, we will:
            // 1: Generate a new set of viable sequences by mutating the existing population.
            // 2: Execute each viable sequence and caputure results.
            // 3: Bucket the results.
            // 4: Cull duplicates.
            // 5: Repeat with the new population.
            for (int generation = 0; generation < 100; generation++)
            {
                Console.WriteLine("\n\n----------------------------------------------------------------------------------");
                Console.WriteLine($"Generation {generation}");
                Console.WriteLine("----------------------------------------------------------------------------------");

                int popCount = population.Population.Count; // Store since we will be growing list.
                for (int seed = 0; seed < popCount; ++seed)
                {

                    // Combine the starting dictionary and the results of this request sequence.
                    List<TokenCollection> seedTokens;
                    if (population.Population[seed].GetResults() == null)
                    {
                        seedTokens = new List<TokenCollection>();
                        seedTokens.Add(new TokenCollection(startingData));
                    }
                    else
                    {
                        seedTokens = population.Population[seed].GetResults();
                    }

                    // Generate candidate request sequences.
                    int candidateNumber = 0;
                    foreach (IGenerator generator in generators)
                    {
                        foreach (RequestSequence candidate in generator.Generate(population.Endpoints, population.Population[seed], seedTokens))
                        {
                            Console.WriteLine($"Generation {generation}, Seed {seed}, Candidate {++candidateNumber}:");
                            Console.WriteLine(candidate.ToString());

                            // Execute the request sequence.
                            await candidate.Execute(client, responseTokenizers, startingData);
                            Response lastResponse = candidate.GetLastResponse();

                            string resultSummary = lastResponse.Content.Substring(0, Math.Min(80, lastResponse.Content.Length));
                            Console.WriteLine($"Result: {lastResponse.Status} : {resultSummary}");

                            // Add a response to the population. If it looks interesting, we will look at it later.
                            population.AddResponse(candidate, candidate.GetLastResult());
                        }
                    }
                }

                population.MinimizePopulation();
                Console.WriteLine(@"-------------------------------------------------------------------------");
                Console.WriteLine(@"                                 Summary                                 ");
                Console.WriteLine(@"-------------------------------------------------------------------------");
                Console.WriteLine(population.Summary());
            }
        }

        public static List<RequestResponsePair> InitializeEndpoints()
        {
            return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop\", @"http://localhost");
        }
    }
}
