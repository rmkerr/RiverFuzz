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
using Database;
using System.Diagnostics;
using Database.Entities;

namespace Fuzz
{
    class Program
    {
        static bool production = false;

        static async Task Main(string[] args)
        {
            // Set up a database connection to store the results.
            DatabaseHelper databaseHelper = new DatabaseHelper("riverfuzz", production);
            databaseHelper.DeleteDatabase();
            databaseHelper.CreateDatabase();

            // Set up HttpClient.
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(2);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Load all response tokenizers.
            List<IResponseTokenizer> responseTokenizers = new List<IResponseTokenizer>();
            responseTokenizers.Add(new JsonTokenizer());
            responseTokenizers.Add(new BearerTokenizer());
            responseTokenizers.Add(new HtmlFormTokenizer());
            //responseTokenizers.Add(new CookieTokenizer());

            // Load all request tokenizers.
            List<IRequestTokenizer> requestTokenizers = new List<IRequestTokenizer>();
            requestTokenizers.Add(new JsonTokenizer());
            requestTokenizers.Add(new QueryTokenizer());
            requestTokenizers.Add(new BearerTokenizer());
            requestTokenizers.Add(new KnownUrlArgumentTokenizer());
            requestTokenizers.Add(new HtmlFormTokenizer());
            requestTokenizers.Add(new CookieTokenizer());

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("log", "user", "", Types.String));
            startingData.Add(new JsonToken("pwd", "43isDOT6OMbe", "", Types.String));

            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\web_store.txt", 3));
            //generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\xss_payloads_many.txt", 10));
            generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\blns.txt", 10));

            PopulationManager population = new PopulationManager();
            foreach (RequestResponsePair endpoint in InitializeEndpoints())
            {
                endpoint.Tokenize(requestTokenizers, responseTokenizers);
                population.AddEndpoint(endpoint, new TokenNameBucketer());
                databaseHelper.AddEndpoint(RequestEntity.FromRequest(endpoint.Request));
            }

            // Record the time we started this run.
            FuzzerGenerationEntity generationInfo = new FuzzerGenerationEntity();
            generationInfo.start_time = DateTime.Now;


            // In each generation, we will:
            // 1: Generate a new set of viable sequences by mutating the existing population.
            // 2: Execute each viable sequence and caputure results.
            // 3: Bucket the results.
            // 4: Keep the shortest sequences from each bucket.
            // 5: Repeat with the new population.
            for (int generation = 0; generation < 20; generation++)
            {
                Console.WriteLine("\n\n----------------------------------------------------------------------------------");
                Console.WriteLine($"Generation {generation}");
                Console.WriteLine("----------------------------------------------------------------------------------");

                Stopwatch generationStopwatch = new Stopwatch();
                generationStopwatch.Start();

                int popCount = population.Population.Count; // Store since we will be growing list.

                // Loops over each existing seed sequence in the population.
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

                    int candidateNumber = 0;
                    // Generate candidate request sequences by mutating that seed.
                    foreach (IGenerator generator in generators)
                    {
                        foreach (RequestSequence candidate in generator.Generate(population.Endpoints, population.Population[seed], seedTokens))
                        {
                            Stopwatch candidateStopwatch = new Stopwatch();
                            candidateStopwatch.Start();

                            // Execute the request sequence.
                            await candidate.Execute(client, responseTokenizers, startingData);

                            // Add a response to the population. If it looks interesting, we will look at it later.
                            population.AddResponse(candidate);

                            candidateStopwatch.Stop();
                            Console.WriteLine($"\tCandidate {candidateNumber++} completed in {candidateStopwatch.ElapsedMilliseconds}ms");
                        }
                    }
                }

                generationStopwatch.Stop();
                Console.WriteLine($"Generation {generation} completed in {generationStopwatch.ElapsedMilliseconds}ms");

                population.MinimizePopulation();
            }

            generationInfo.end_time = DateTime.Now;
            databaseHelper.AddFuzzerGeneration(generationInfo);

            foreach (RequestSequence sequence in population.Population)
            {
                Response? finalResponse = sequence.GetLastResponse();
                if (finalResponse != null)
                {
                    databaseHelper.AddRequestSequence(sequence, generationInfo);
                }     
            }

            
        }

        public static List<RequestResponsePair> InitializeEndpoints()
        {
            //return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop\", @"http://localhost");
            return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\Wordpress\", @"http://192.168.0.220/");
        }
    }
}
