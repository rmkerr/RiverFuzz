using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using HttpTokenize.Bucketers;
using HttpTokenize.Substitutions;
using Generators;
using CaptureParse;

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

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("username", "asdfg@asdfg.com", "", Types.String));
            startingData.Add(new JsonToken("username", "admin", "", Types.String));
            startingData.Add(new JsonToken("password", "asdfg", "", Types.String));
            startingData.Add(new JsonToken("Constant(null)", "\0", "", Types.String));
            startingData.Add(new JsonToken("Constant(-1)", "-1", "", Types.Integer));
            startingData.Add(new JsonToken("Constant(0)", "0", "", Types.Integer));
            startingData.Add(new JsonToken("Constant(1)", "1", "", Types.Integer));
            startingData.Add(new JsonToken("Constant(Int.Max)", "", int.MaxValue.ToString(), Types.Integer));
            startingData.Add(new JsonToken("Constant(Int.Min)", "", int.MinValue.ToString(), Types.Integer));

            List<IGenerator> generators = new List<IGenerator>();
            generators.Add(new BestKnownMatchGenerator());
            generators.Add(new DictionarySubstitutionGenerator(@"C:\Users\Richa\Documents\Tools\Lists\blns.txt", 1));

            Random rand = new Random(); // Used to select generator;

            Dictionary<string, IBucketer> bucketers = new Dictionary<string, IBucketer>();

            List<RequestResponsePair> endpoints = InitializeEndpoints();
            foreach (RequestResponsePair endpoint in endpoints)
            {
                bucketers.Add(endpoint.Request.ToString(), new TokenNameBucketer());
                endpoint.Tokenize(requestTokenizers, responseTokenizers);
            }

            // Start with an initial population of one empty request sequence.
            List<RequestSequence> population = new List<RequestSequence>();
            population.Add(new RequestSequence());
            
            // In each generation, we will:
            // 1: Generate a new set of viable sequences by mutating the existing population.
            // 2: Execute each viable sequence and caputure results.
            // 3: Bucket the results.
            // 4: Cull duplicates.
            // 5: Repeat with the new population.
            for (int generation = 0; generation < 200; generation++)
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
                    IGenerator generator = generators[rand.Next(0, generators.Count)];
                    foreach (RequestSequence candidate in generator.Generate(endpoints, population[seed], seedTokens))
                    {
                        Console.WriteLine($"Generation {generation}, Seed {seed}, Candidate {++candidateNumber}:");
                        Console.WriteLine(candidate.ToString());

                        // Execute the request sequence.
                        List<Response> results = await candidate.Execute(client, responseTokenizers, startingData);

                        // If the response results in a new bucket of responses, add it to the population.
                        if (bucketers[candidate.Get(results.Count - 1).Request.ToString()].Add(results[results.Count-1], results[results.Count - 1].GetResults(responseTokenizers)) &&
                            results[results.Count - 1].Status == System.Net.HttpStatusCode.OK)
                        {
                            // TODO: if bucketer thinks is interesting.
                            population.Add(candidate);
                        }
                    }
                }

                foreach (RequestResponsePair endpoint in endpoints)
                {
                    IBucketer bucketer = bucketers[endpoint.Request.ToString()];
                    List<List<Response>> bucketed = bucketer.Bucketize();

                    Console.WriteLine($"\nEndpoint {endpoint.Request.ToString()}");
                    Console.WriteLine($"\t{bucketed.Count} buckets.");
                    foreach (List<Response> bucket in bucketed)
                    {
                        //population.Add(bucket[0]); // TODO: Prefer shorter paths.
                        string summary = bucket[0].Content.Substring(0, Math.Min(50, bucket[0].Content.Length));
                        Console.WriteLine($"\t{bucket[0].Status} : {summary}");
                    }
                }
            }
        }

        public static List<RequestResponsePair> InitializeEndpoints()
        {
            return BurpSavedParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop\", @"http://localhost");
            //return TextCaptureParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop", @"http://localhost");
        }
    }
}
