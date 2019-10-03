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
            TokenCollection loginTokens = loginUser.GetRequirements(request_tokenizers);
            Stage login = new Stage(loginUser);

            string initializeCartJson = "{\"ProductId\":24,\"BasketId\":\"20\",\"quantity\":1}";
            Request initializeCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post, initializeCartJson);
            initializeCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**");
            TokenCollection initializeTokens = initializeCart.GetRequirements(request_tokenizers);
            Stage initialize = new Stage(initializeCart);
            initialize.Substitutions.Add(new SubstituteNamedToken(initializeTokens.GetByName("BearerToken"), "BearerToken", Types.BearerToken));
            initialize.Substitutions.Add(new SubstituteNamedToken(initializeTokens.GetByName("BasketId"), "bid", Types.Integer));

            string addToCartJson = "{\"quantity\":2}";
            Request addToCart = new Request(new Uri(@"http://localhost/api/BasketItems/16/"), HttpMethod.Put, addToCartJson);
            addToCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**");
            TokenCollection addTokens = addToCart.GetRequirements(request_tokenizers);
            Stage addItem = new Stage(addToCart);
            addItem.Substitutions.Add(new SubstituteNamedToken(addTokens.GetByName("BearerToken"), "BearerToken", Types.BearerToken));

            List<Request> endpoints = new List<Request>();
            endpoints.Add(loginUser);
            endpoints.Add(initializeCart);
            endpoints.Add(addToCart);

            RequestSequence sequence = new RequestSequence();
/*            sequence.Add(login);
            sequence.Add(initialize);
            sequence.Add(addItem);*/

            Tuple<List<Response>, TokenCollection> results = await sequence.Execute(client, responseTokenizers);

            TokenCollection startingData = new TokenCollection();
            startingData.Add(new JsonToken("email", "asdf@asdf.com", Types.String));
            startingData.Add(new JsonToken("password", "123456", Types.String));

            TypeMatchGenerator generator = new TypeMatchGenerator();
            int i = 0;
            foreach (RequestSequence candidate in generator.Generate(endpoints, sequence, startingData, request_tokenizers))
            {
                Console.WriteLine($"Candidate {++i}");
                foreach (Stage stage in candidate)
                {
                    Console.WriteLine(stage.Request.Url);
                    foreach (ISubstitution sub in stage.Substitutions)
                    {
                        Console.WriteLine('\t' + sub.ToString());
                    }
                }
            }

            IBucketer bucketer = new TokenNameBucketer();
            bucketer.Responses.AddRange(results.Item1);

            List<List<Response>> bucketed = bucketer.Bucketize();
            Console.WriteLine($"\n{bucketed.Count} buckets.");
            foreach(List<Response> bucket in bucketed)
            {
                Console.WriteLine($"{bucket[0].Status} : {bucket[0].Content}");
            }
        }
    }
}
