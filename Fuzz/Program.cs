using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers
;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using HttpTokenize.Bucketers;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string addUserJson = "{\"email\":\"asdf@asdf.com\",\"password\":\"123456\",\"passwordRepeat\":\"123456\",\"securityQuestion\":{\"id\":2,\"question\":\"Your eldest siblings middle name?\",\"createdAt\":\"2019-09-27T06:18:54.480Z\",\"updatedAt\":\"2019-09-27T06:18:54.480Z\"},\"securityAnswer\":\"asdf\"}";
            Request addUserRequest = new Request(new Uri(@"http://localhost/api/Users/"), HttpMethod.Post, addUserJson);

            string initializeCartJson = "{\"ProductId\":24,\"BasketId\":\"20\",\"quantity\":1}";
            Request initializeCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post, initializeCartJson);
            initializeCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**");

            string loginUserJson = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";
            Request loginUser = new Request(new Uri(@"http://localhost/rest/user/login/"), HttpMethod.Post, loginUserJson);

            string addToCartJson = "{\"quantity\":2}";
            Request addToCart = new Request(new Uri(@"http://localhost/api/BasketItems/16/"), HttpMethod.Put, addToCartJson);
            addToCart.Headers.Add("Authorization", "Bearer **DUMMYVAL**");

            List<Request> knownRequests = new List<Request>();
            knownRequests.Add(addUserRequest);
            knownRequests.Add(initializeCart);
            knownRequests.Add(loginUser);

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

            // Force a login.
            HttpResponseMessage response = await client.SendAsync(loginUser.GenerateRequest());
            Response parsedResponse = new Response(response.StatusCode, await response.Content.ReadAsStringAsync());

            // Parse and iterate through all response tokens from the login.
            TokenCollection requestResults = parsedResponse.GetResults(responseTokenizers);

            Random rand = new Random(0);

            Request initializeCartUpdated = initializeCart.Clone();
            TokenCollection requirements = initializeCartUpdated.GetRequirements(request_tokenizers);
            requirements.GetByName("BearerToken")?.ReplaceValue(initializeCartUpdated, requestResults.GetByName("BearerToken").Value);
            requirements.GetByName("BasketId")?.ReplaceValue(initializeCartUpdated, requestResults.GetByName("bid").Value);
            requirements.GetByName("quantity")?.ReplaceValue(initializeCartUpdated, rand.Next(-5, 5).ToString());

            // Send another request.
            response = await client.SendAsync(initializeCartUpdated.GenerateRequest());
            parsedResponse = new Response(response.StatusCode, await response.Content.ReadAsStringAsync());

            IBucketer bucketer = new TokenNameBucketer();

            // Copy over the results to a new request.
            for (int i = 0; i < 10; ++i)
            {
                Request addToCartUpdated = addToCart.Clone();
                requirements = addToCartUpdated.GetRequirements(request_tokenizers);
                requirements.GetByName("BearerToken")?.ReplaceValue(addToCartUpdated, requestResults.GetByName("BearerToken").Value);
                requirements.GetByName("quantity")?.ReplaceValue(addToCartUpdated, rand.Next(-50, 50).ToString());

                // Send another request.
                response = await client.SendAsync(addToCartUpdated.GenerateRequest());
                parsedResponse = new Response(response.StatusCode, await response.Content.ReadAsStringAsync());

                bucketer.Responses.Add(parsedResponse);
            }

            List<List<Response>> bucketed = bucketer.Bucketize();
            Console.WriteLine($"{bucketed.Count} buckets.");
            foreach(List<Response> bucket in bucketed)
            {
                Console.WriteLine($"{bucket[0].Status} : {bucket[0].Content}");
            }
        }
    }
}
