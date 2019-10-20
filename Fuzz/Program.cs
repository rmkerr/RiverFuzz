using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Generators;
using CaptureParse;
using PopulationManager.Bucketers;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Set up HttpClient. TODO: Break requirement that we set content type json
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(1);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Load all response tokenizers.
            // TODO: Fix multiple headers with the same name, add CookieTokenizer
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
            requestTokenizers.Add(new CookieTokenizer());

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("username", "asdfg@asdfg.com", "", Types.String));
            startingData.Add(new JsonToken("password", "asdfg", "", Types.String));

            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\web_store.txt", 3));
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\xss_payloads_many.txt", 10));
            generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\blns.txt", 10));

            Dictionary<Request, IBucketer> bucketers = new Dictionary<Request, IBucketer>();

            List<RequestResponsePair> endpoints = InitializeEndpoints();
            foreach (RequestResponsePair endpoint in endpoints)
            {
                bucketers.Add(endpoint.Request.OriginalEndpoint, new TokenNameBucketer());
                endpoint.Tokenize(requestTokenizers, responseTokenizers);
            }

            // TODO: Implement per-endpoint bucketing.
            //bucketers[@"GET http://localhost/rest/user/whoami"] = new ExactStringBucketer();

            // Start with an initial population of one empty request sequence.
            List<RequestSequence> population = new List<RequestSequence>();
            population.Add(new RequestSequence());
            
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

                int popCount = population.Count; // Store since we will be growing list.
                for (int seed = 0; seed < popCount; ++seed)
                {

                    // Combine the starting dictionary and the results of this request sequence.
                    List<TokenCollection> seedTokens;
                    if (population[seed].GetResults() == null)
                    {
                        seedTokens = new List<TokenCollection>();
                        seedTokens.Add(new TokenCollection(startingData));
                    }
                    else
                    {
                        seedTokens = population[seed].GetResults();
                    }

                    // Generate candidate request sequences.
                    int candidateNumber = 0;
                    foreach (IGenerator generator in generators)
                    {
                        foreach (RequestSequence candidate in generator.Generate(endpoints, population[seed], seedTokens))
                        {
                            Console.WriteLine($"Generation {generation}, Seed {seed}, Candidate {++candidateNumber}:");
                            Console.WriteLine(candidate.ToString());

                            // Execute the request sequence.
                            List<Response> results = await candidate.Execute(client, responseTokenizers, startingData);

                            string resultSummary = results[results.Count - 1].Content.Substring(0, Math.Min(80, results[results.Count - 1].Content.Length));
                            Console.WriteLine($"Result: {results[results.Count - 1].Status} : {resultSummary}");

                            // If the response results in a new bucket of responses, add it to the population.
                            if (bucketers[candidate.Get(results.Count - 1).Request.OriginalEndpoint].Add(results[results.Count - 1], results[results.Count - 1].GetResults(responseTokenizers)) &&
                                results[results.Count - 1].Status == System.Net.HttpStatusCode.OK)
                            {
                                population.Add(candidate); // TODO: Prefer shorter paths.
                            }
                        }
                    }
                }

                foreach (RequestResponsePair endpoint in endpoints)
                {
                    IBucketer bucketer = bucketers[endpoint.Request.OriginalEndpoint];
                    List<List<Response>> bucketed = bucketer.Bucketize();

                    Console.WriteLine($"\nEndpoint {endpoint.Request.ToString()}");
                    Console.WriteLine($"\t{bucketed.Count} buckets.");
                    foreach (List<Response> bucket in bucketed)
                    {
                        string summary = bucket[0].Content.Substring(0, Math.Min(80, bucket[0].Content.Length));
                        Console.WriteLine($"\t{bucket[0].Status} : {summary}");
                    }
                }
            }
        }

        public static List<RequestResponsePair> InitializeEndpoints()
        {
            return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop\", @"http://localhost");
        }
    }
}
