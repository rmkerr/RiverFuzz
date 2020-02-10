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
using ProjectSpecific;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Fuzz
{
    public class Program
    {
        static bool production = false;

        static async Task Main(string[] args)
        {
            string fuzzerConfig = System.IO.File.ReadAllText(@"fuzz.json");
            JObject config = JObject.Parse(fuzzerConfig);
            
            await Fuzz(config);
        }

        public static async Task Fuzz(JObject config)
        {
            // Set up a database connection to store the results.
            DatabaseHelper databaseHelper = new DatabaseHelper("riverfuzz", production);
            if (config.Value<bool?>("ResetDatabase") ?? false)
            {
                databaseHelper.DeleteDatabase();
                databaseHelper.CreateDatabase();
            }

            // Set up HttpClient.
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            handler.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMilliseconds(1000);
            // client.DefaultRequestHeaders.Add("Accept", "application/json");

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

            // OWASP juice shop
            startingData.Add(new HttpTokenize.Tokens.JsonToken("user", "asdfg@asdfg.com", "", Types.String));
            startingData.Add(new HttpTokenize.Tokens.JsonToken("password", "asdfg", "", Types.String));

            // Generators take a sequence and modify it.
            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            generators.Add(new RemoveTokenGenerator(5));
            generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\blns.txt", 10));

            // Parse the list of endpoints we should include in this run, then load them.
            DatabaseParse databaseParse = new DatabaseParse(databaseHelper, config.Value<string>("Target"));
            List<int> endpointIds = config["TargetEndpoints"].Select(x => (int)x).ToList();
            List<KnownEndpoint> endpoints = await databaseParse.LoadEndpointsById(endpointIds);

            // Add the endpoints to the population and set up bucketers.
            PopulationManager population = new PopulationManager();
            foreach (KnownEndpoint endpoint in endpoints)
            {
                endpoint.Tokenize(requestTokenizers, responseTokenizers);

                if (endpoint.Request.Url.AbsolutePath.Contains("login"))
                {
                    // The bucketer sorts based on the token names produced by each result, and ignores
                    // the values. However, we can specify individual token names we are interested in
                    // the value of. In this case, we can specifically check which user is logged in.
                    population.AddEndpoint(endpoint, new TokenNameBucketer(new string[] { "umail" }));
                }
                else if (endpoint.Request.Url.AbsolutePath.Contains("addresss"))
                {
                    population.AddEndpoint(endpoint, new StatusCodeBucketer());
                }
                else
                {
                    population.AddEndpoint(endpoint, new TokenNameBucketer());
                }

                // If the endpoint is not already in the database, add it.
                if (endpoint.Request.Id == null)
                {
                    databaseHelper.AddEndpoint(RequestEntity.FromRequest(endpoint.Request));
                }
            }

            // Record the time we started this run.
            FuzzerRunEntity runInfo = new FuzzerRunEntity();
            runInfo.name = config.Value<string?>("RunName") ?? "Untitled Fuzzer Run";
            runInfo.start_time = DateTime.Now;
            List<FuzzerGenerationEntity> generationInfo = new List<FuzzerGenerationEntity>();

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
                
                generationInfo.Add(new FuzzerGenerationEntity
                {
                    population_size = population.Population.Count,
                    run_position = generation,
                    execution_time = generationStopwatch.Elapsed
                });

                Console.WriteLine($"Population size: {population.Population.Count}");
            }

            runInfo.end_time = DateTime.Now;
            databaseHelper.AddFuzzerRun(runInfo);
            foreach (FuzzerGenerationEntity entity in generationInfo)
            {
                entity.run_id = runInfo.id.Value;
                databaseHelper.AddFuzzerGeneration(entity);
            }

            foreach (RequestSequence sequence in population.Population)
            {
                Response? finalResponse = sequence.GetLastResponse();
                if (finalResponse != null)
                {
                    await databaseHelper.AddRequestSequence(sequence, runInfo);
                }     
            }

            
        }

        public static List<KnownEndpoint> InitializeEndpoints(string endpoint_path, string host)
        {
            return BurpSavedParse.LoadRequestsFromDirectory(endpoint_path, host);
            //return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\Wordpress\wp-json", @"http://192.168.43.232");
            //return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\Moodle", @"http://10.0.0.197");
        }
    }
}
