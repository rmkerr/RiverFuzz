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

            // Load all request tokenizers.
            List<IRequestTokenizer> request_tokenizers = new List<IRequestTokenizer>();
            request_tokenizers.Add(new JsonTokenizer());
            request_tokenizers.Add(new QueryTokenizer());
            request_tokenizers.Add(new BearerTokenizer());
            request_tokenizers.Add(new KnownUrlArgumentTokenizer());

            List<Request> endpoints = InitializeEndpoints();

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("const(email)", "asdf@asdf.com", Types.String));
            startingData.Add(new JsonToken("const(password)", "123456", Types.String));
            startingData.Add(new JsonToken("Constant(-1)", "-1", Types.Integer));
            startingData.Add(new JsonToken("Constant(0)", "0", Types.Integer));
            startingData.Add(new JsonToken("Constant(1)", "1", Types.Integer));

            BestKnownMatchGenerator generator = new BestKnownMatchGenerator();
            IBucketer bucketer = new TokenNameBucketer();

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
                    int candidateNumber = 0;
                    TokenCollection seedTokens = new TokenCollection(startingData);
                    if (population[seed].GetResults() != null)
                    {
                        seedTokens.Add(population[seed].GetResults());
                    }

                    // Generate viable request sequences.
                    foreach (RequestSequence candidate in generator.Generate(endpoints, population[seed], seedTokens, request_tokenizers))
                    {
                        Console.WriteLine($"Generation {generation}, Seed {seed}, Candidate {++candidateNumber}:");
                        Console.WriteLine(candidate.ToString());

                        // Execute the request sequence.
                        List<Response> results = await candidate.Execute(client, responseTokenizers, startingData);

                        // If the response results in a new bucket of responses, add it to the population.
                        if (bucketer.Add(results[results.Count-1], results[results.Count - 1].GetResults(responseTokenizers)) &&
                            results[results.Count - 1].Status == System.Net.HttpStatusCode.OK)
                        {
                            // TODO: if bucketer thinks is interesting.
                            population.Add(candidate);
                        }
                    }
                }

                List<List<Response>> bucketed = bucketer.Bucketize();

                Console.WriteLine($"\n{bucketed.Count} buckets.");
                foreach (List<Response> bucket in bucketed)
                {
                    //population.Add(bucket[0]); // TODO: Prefer shorter paths.
                    Console.WriteLine($"{bucket[0].Status} : {bucket[0].Content}");
                }
            }
        }

        public static List<Request> InitializeEndpoints()
        {
            return TextCaptureParse.LoadRequestsFromDirectory(@"C:\Users\Richa\Documents\RiverFuzzResources\JuiceShop", @"http://localhost");
        }
    }
}
