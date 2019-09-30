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
            // Set up known requests. TODO: Load this from a file or set of strings.
            Request addUserRequest = new Request();
            addUserRequest.Url = new Uri(@"http://localhost/api/Users/");
            addUserRequest.Method = HttpMethod.Post;
            addUserRequest.Content = "{\"email\":\"asdf@asdf.com\",\"password\":\"123456\",\"passwordRepeat\":\"123456\",\"securityQuestion\":{\"id\":2,\"question\":\"Your eldest siblings middle name?\",\"createdAt\":\"2019-09-27T06:18:54.480Z\",\"updatedAt\":\"2019-09-27T06:18:54.480Z\"},\"securityAnswer\":\"asdf\"}";

            Request initializeCart = new Request();
            initializeCart.Url = new Uri(@"http://localhost/api/BasketItems/");
            initializeCart.Method = HttpMethod.Post;
            initializeCart.Content = "{\"ProductId\":24,\"BasketId\":\"20\",\"quantity\":1}";
            initializeCart.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTc0MzEwNSwiZXhwIjoxNTY5NzYxMTA1fQ.Hwjir8myg-rWOpEXlpD-YpA785rY3yRJH24SQkISBYW1MlxnIFmFera3Q48E0VEtlcGSpViBfUCLBFMqMGDdfp5-ujzRrRTq0pHbVjMWqnAMygheO3KYxpvGyY2o1LbAx4EOUksdIGwpxnTRMugVudOWPzZFr89uvKj-Iet6Ig0");

            Request loginUser = new Request();
            loginUser.Url = new Uri(@"http://localhost/rest/user/login/");
            loginUser.Method = HttpMethod.Post;
            loginUser.Content = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";

            Request addToCart = new Request();
            addToCart.Url = new Uri(@"http://localhost/api/BasketItems/82/");
            addToCart.Method = HttpMethod.Put;
            addToCart.Content = "{\"quantity\":2}";
            addToCart.Headers.Add("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTc0MzEwNSwiZXhwIjoxNTY5NzYxMTA1fQ.Hwjir8myg-rWOpEXlpD-YpA785rY3yRJH24SQkISBYW1MlxnIFmFera3Q48E0VEtlcGSpViBfUCLBFMqMGDdfp5-ujzRrRTq0pHbVjMWqnAMygheO3KYxpvGyY2o1LbAx4EOUksdIGwpxnTRMugVudOWPzZFr89uvKj-Iet6Ig0");


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
                requirements.GetByName("quantity")?.ReplaceValue(addToCartUpdated, rand.Next(-10, 30).ToString());

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
