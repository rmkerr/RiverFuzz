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

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // string addUserJson = "{\"email\":\"asdf@asdf.com\",\"password\":\"123456\",\"passwordRepeat\":\"123456\",\"securityQuestion\":{\"id\":2,\"question\":\"Your eldest siblings middle name?\",\"createdAt\":\"2019-09-27T06:18:54.480Z\",\"updatedAt\":\"2019-09-27T06:18:54.480Z\"},\"securityAnswer\":\"asdf\"}";
            // Request addUserRequest = new Request(new Uri(@"http://localhost/api/Users/"), HttpMethod.Post, addUserJson);

            // Set up HttpClient. TODO: Break requirement that we set content type json
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(5);
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

            string loginUserJson = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";
            Request loginUser = new Request(new Uri(@"http://localhost/rest/user/login/"), HttpMethod.Post, loginUserJson);

            string initializeCartJson = "{\"ProductId\":24,\"BasketId\":\"20\",\"quantity\":1}";
            Request initializeCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post, initializeCartJson);
            initializeCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**"); // Real tokens are v long.

            string addToCartJson = "{\"quantity\":2}";
            Request addToCart = new Request(new Uri(@"http://localhost/api/BasketItems/16/"), HttpMethod.Put, addToCartJson);
            addToCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**"); // Real tokens are v long.

            List<Request> endpoints = new List<Request>();
            endpoints.Add(loginUser);
            endpoints.Add(initializeCart);
            endpoints.Add(addToCart);

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("email", "asdf@asdf.com", Types.String));
            startingData.Add(new JsonToken("password", "123456", Types.String));

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
            for (int generation = 0; generation < 10; generation++)
            {
                Console.WriteLine("\n\n----------------------------------------------------------------------------------");
                Console.WriteLine($"Generation {generation}");
                Console.WriteLine("----------------------------------------------------------------------------------");

                int popCount = population.Count; // Store since we will be growing list.
                for (int seed = 0; seed < popCount; ++seed)
                {
                    int candidateNumber = 0;
                    TokenCollection seedTokens = new TokenCollection(startingData);
                    if (population[seed].GetResults() != null)
                    {
                        seedTokens.Add(population[seed].GetResults());
                    }
                    foreach (RequestSequence candidate in generator.Generate(endpoints, population[seed], seedTokens, request_tokenizers))
                    {
                        Console.WriteLine($"Generation {generation}, Seed {seed}, Candidate {++candidateNumber}:");
                        Console.WriteLine(candidate.ToString());

                        List<Response> results = await candidate.Execute(client, responseTokenizers, startingData);

                        if (bucketer.Add(results[results.Count-1], results[results.Count - 1].GetResults(responseTokenizers)))
                        {
                            // TODO: if bucketer thinks is interesting.
                            population.Add(candidate);
                        }
                    }
                }

                List<List<Response>> bucketed = bucketer.Bucketize();
                //population.Clear();

                Console.WriteLine($"\n{bucketed.Count} buckets.");
                foreach (List<Response> bucket in bucketed)
                {
                    //population.Add(bucket[0]); // TODO: Prefer shorter paths.
                    Console.WriteLine($"{bucket[0].Status} : {bucket[0].Content}");
                }
            }
        }
    }
}
