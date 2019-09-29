using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers
;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Request request1 = new Request();
            request1.Url = new Uri(@"http://localhost/api/Users/");
            request1.Method = HttpMethod.Post;
            request1.Content = "{\"email\":\"asdf@asdf.com\",\"password\":\"123456\",\"passwordRepeat\":\"123456\",\"securityQuestion\":{\"id\":2,\"question\":\"Your eldest siblings middle name?\",\"createdAt\":\"2019-09-27T06:18:54.480Z\",\"updatedAt\":\"2019-09-27T06:18:54.480Z\"},\"securityAnswer\":\"asdf\"}";

            Request request2 = new Request();
            request2.Url = new Uri(@"http://localhost/api/BasketItems");
            request2.Method = HttpMethod.Post;
            request2.Content = "{\"ProductId\":24,\"BasketId\":\"7\",\"quantity\":1}";

            // Set up HttpClient. TODO: Break requirement that we hold bearer token and manually set content type json
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTcxODgxOCwiZXhwIjoxNTY5NzM2ODE4fQ.q9zs_-mfnFpWAOBhltgaVtQU115ZgiaoiOlKsy0qUqNNOZQNUo4IeXT0M2bglvIpW1qteixG_LdntAScCsNRLRfVkVlE87-VP5yAL09CdJwYQfhFwyPoZRrT_nW_JXnM6_r4ST6tnJ3Fq91-ZbaRdjwtaFT112fOT5LoyLamsbM");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            List<IToken> currentSequenceTokens = new List<IToken>();
            List<IToken> request2Tokens = new List<IToken>();

            // Extract request tokens
            List<IRequestTokenizer> request_tokenizers = new List<IRequestTokenizer>();
            request_tokenizers.Add(new JsonTokenizer());
            request_tokenizers.Add(new QueryTokenizer());
            foreach (IRequestTokenizer tokenizer in request_tokenizers)
            {
                currentSequenceTokens.AddRange(tokenizer.ExtractTokens(request1));
                request2Tokens.AddRange(tokenizer.ExtractTokens(request2));
            }

            HttpResponseMessage response = await client.SendAsync(request1.GenerateRequest());
            Response response1 = new Response(response.StatusCode, await response.Content.ReadAsStringAsync());

            // Extract response tokens
            List<IResponseTokenizer> response_tokenizers = new List<IResponseTokenizer>();
            response_tokenizers.Add(new JsonTokenizer());
            foreach (IResponseTokenizer tokenizer in response_tokenizers)
            {
                currentSequenceTokens.AddRange(tokenizer.ExtractTokens(response1));
            }

            foreach (IToken token1 in currentSequenceTokens)
            {
                foreach (IToken token2 in request2Tokens)
                {
                    Console.WriteLine("\n-----------------------------------------------------------");
                    Console.WriteLine($"Token 1: {token1}");
                    Console.WriteLine($"Token 2: {token2}");

                    Console.WriteLine("Replacement of value.");
                    Request updated_request = request2.Clone();
                    await token2.ReplaceValue(updated_request, token1.Value);
                    Console.WriteLine(updated_request.Content);

                    try
                    {
                        response = await client.SendAsync(updated_request.GenerateRequest());
                        Console.WriteLine(response.StatusCode);

                        Response response2 = new Response(response.StatusCode, await response.Content.ReadAsStringAsync());

                        Console.WriteLine(response2.Content);
                    }
                    catch
                    {
                        Console.WriteLine("Timeout.");
                    }
                }
            }
        }
    }
}
