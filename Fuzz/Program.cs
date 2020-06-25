using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Generators;
using Population.Bucketers;
using Population;
using Database;
using System.Diagnostics;
using Database.Entities;
using Newtonsoft.Json.Linq;
using System.Linq;
using CaptureParse.Parsers;
using CaptureParse.Loaders;
using Database.Repositories;

namespace Fuzz
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string fuzzerConfig = System.IO.File.ReadAllText(@"fuzz.json");
            JObject config = JObject.Parse(fuzzerConfig);
            
            await Fuzz(config);
        }

        public static async Task Fuzz(JObject config)
        {
            // Set up a database connection to store the results.
            // TODO: Move this hardcoded connection string to a config file.
            FuzzerRepository databaseHelper = new FuzzerRepository("Server=db;Database=riverfuzz;Port=5432;User Id=postgres;Password='3r!T8*Qb8YNFlG8Eb8u';");
            if (config.Value<bool?>("ResetDatabase") ?? false)
            {
                databaseHelper.InitializeDatabase();
            }

            // Set up HttpClient.
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            handler.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMilliseconds(1000);

            // Load all response tokenizers.
            List<IResponseTokenizer> responseTokenizers = new List<IResponseTokenizer>();
            responseTokenizers.Add(new JsonTokenizer());
            responseTokenizers.Add(new BearerTokenizer());
            responseTokenizers.Add(new HtmlFormTokenizer());
            responseTokenizers.Add(new CookieTokenizer());

            // Load all request tokenizers.
            List<IRequestTokenizer> requestTokenizers = new List<IRequestTokenizer>();
            requestTokenizers.Add(new JsonTokenizer());
            requestTokenizers.Add(new QueryTokenizer());
            requestTokenizers.Add(new BearerTokenizer());
            requestTokenizers.Add(new KnownUrlArgumentTokenizer());
            requestTokenizers.Add(new HtmlFormTokenizer());
            requestTokenizers.Add(new CookieTokenizer());

            TokenCollection startingData = new TokenCollection();

            // Generators take a sequence and modify it.
            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            generators.Add(new RemoveTokenGenerator(5));

            List<string> dictionary = await databaseHelper.GetAllDictionaryEntries();
            generators.Add(new DictionarySubstitutionGenerator(dictionary, 10));

            // Parse the list of endpoints we should include in this run, then load them.
            DatabaseLoader databaseParse = new DatabaseLoader(databaseHelper, config.Value<string>("Target"));
            List<int> endpointIds = config["TargetEndpoints"].Select(x => (int)x).ToList();
            List<KnownEndpoint> endpoints = await databaseParse.LoadEndpointsById(endpointIds);

            // Add the endpoints to the population and set up bucketers.
            PopulationManager population = new PopulationManager();
            foreach (KnownEndpoint endpoint in endpoints)
            {
                endpoint.Tokenize(requestTokenizers, responseTokenizers);
                population.AddEndpoint(endpoint, new TokenNameBucketer());

                // If the endpoint is not already in the database, add it.
                if (endpoint.Request.Id == null)
                {
                    databaseHelper.AddEndpoint(KnownEndpointEntity.FromRequest(endpoint.Request));
                }
            }

            // Record the time we started this run.
            FuzzerRunEntity runInfo = new FuzzerRunEntity();
            runInfo.name = config.Value<string?>("RunName") ?? "Untitled Fuzzer Run";
            runInfo.start_time = DateTime.Now;
            runInfo.end_time = DateTime.MaxValue;
            databaseHelper.AddFuzzerRun(runInfo);

            // TimeSpan used to stop the fuzzer.
            TimeSpan timeLimit = TimeSpan.FromMinutes(config.Value<int>("ExecutionTime"));
            Stopwatch runTime = new Stopwatch();
            runTime.Start();

            // In each generation, we will:
            // 1: Generate a new set of viable sequences by mutating the existing population.
            // 2: Execute each viable sequence and caputure results.
            // 3: Bucket the results.
            // 4: Keep the shortest sequences from each bucket.
            // 5: Repeat with the new population.
            for (int generation = 0; runTime.ElapsedMilliseconds < timeLimit.TotalMilliseconds; generation++)
            {
                Console.WriteLine("\n\n----------------------------------------------------------------------------------");
                Console.WriteLine($"Generation {generation}");
                Console.WriteLine("----------------------------------------------------------------------------------");

                Stopwatch generationStopwatch = new Stopwatch();
                generationStopwatch.Start();

                int popCount = population.Population.Count; // Store since we will be growing list.
                int requestCount = 0; // Used for performance measurement.

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

                            List<Response>? responses = candidate.GetResponses();
                            if (responses != null)
                            {
                                requestCount += responses.Count;
                            }

                            candidateStopwatch.Stop();

                            if (candidateStopwatch.ElapsedMilliseconds > 500)
                            {
                                Console.WriteLine($"\tWARNING: Long running candidate {candidateNumber++} completed in {candidateStopwatch.ElapsedMilliseconds}ms");
                            }
                        }
                    }
                }
                // await resetHelper.Reset(client);

                population.MinimizePopulation();

                generationStopwatch.Stop();
                Console.WriteLine($"Generation {generation} completed in {generationStopwatch.ElapsedMilliseconds}ms");

                FuzzerGenerationEntity genEntity = new FuzzerGenerationEntity {
                    population_size = population.Population.Count,
                    run_position = generation,
                    execution_time = generationStopwatch.Elapsed,
                    executed_requests = requestCount,
                    run_id = runInfo.id.Value
                };
                databaseHelper.AddFuzzerGeneration(genEntity);

                Console.WriteLine($"Population size: {population.Population.Count}");
                Console.WriteLine($"Requests per second: {requestCount / generationStopwatch.Elapsed.TotalSeconds}");
            }

            runInfo.end_time = DateTime.Now;
            databaseHelper.UpdateFuzzerRunEndTime(runInfo);

            foreach (RequestSequence sequence in population.Population)
            {
                Response? finalResponse = sequence.GetLastResponse();
                if (finalResponse != null)
                {
                    await databaseHelper.AddRequestSequence(sequence, runInfo);
                }     
            }
        }
    }
}
