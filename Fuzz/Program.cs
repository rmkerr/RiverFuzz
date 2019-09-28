using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Request request1 = new Request();
            request1.Url = new Uri("http://localhost/api/Users/");
            request1.Method = HttpMethod.Post;
            request1.Content = new StringContent("{\"email\":\"asdf@asdf.com\",\"password\":\"123456\",\"passwordRepeat\":\"123456\",\"securityQuestion\":{\"id\":2,\"question\":\"Your eldest siblings middle name?\",\"createdAt\":\"2019-09-27T06:18:54.480Z\",\"updatedAt\":\"2019-09-27T06:18:54.480Z\"},\"securityAnswer\":\"asdf\"}");

            Request request2 = new Request();
            request2.Url = new Uri("http://localhost/api/BasketItems");
            request2.Method = HttpMethod.Post;
            request2.Content = new StringContent("{\"ProductId\":24,\"BasketId\":\"5\",\"quantity\":1}");

            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = false;
            HttpClient client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(1);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTY5NzkzMiwiZXhwIjoxNTY5NzE1OTMyfQ.KvyCkDZbaf1PQeiXVV8KjJmU-AkmOuUDIe5xLL-2UdDBlTExwFHrmoZDhcb1hKO_kKOW-i1uhUbitTd4hl_x7n_XUyY-vNq1HnRU6ZL6I_XtFq8B8WbqFe2dUXhXb_i17z3MtfIYxxksNWEckt6BDzllEPCvYfg5tjkDYm9-nMY");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            HttpResponseMessage responseMessage = await client.SendAsync(request2.GenerateRequest());
            Console.WriteLine(responseMessage.StatusCode);

            foreach (IToken token1 in request1.GetTokens())
            {
                foreach (IToken token2 in request2.GetTokens())
                {
                    Console.WriteLine($"\nToken 1: {token1}");
                    Console.WriteLine($"Token 2: {token2}");

                    Console.WriteLine("Full replacement of name and value.");
                    Request updated_request = request2.Clone();
                    await token2.ReplaceToken(updated_request, token1);
                    Console.WriteLine(await updated_request.Content.ReadAsStringAsync());

                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(updated_request.GenerateRequest());
                        Console.WriteLine(response.StatusCode);
                    }
                    catch
                    {
                        Console.WriteLine("Timeout.");
                    }

                    Console.WriteLine("Replacement of name.");
                    updated_request = request2.Clone();
                    await token2.ReplaceName(updated_request, token1.Name);
                    Console.WriteLine(await updated_request.Content.ReadAsStringAsync());

                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(updated_request.GenerateRequest());
                        Console.WriteLine(response.StatusCode);
                    }
                    catch
                    {
                        Console.WriteLine("Timeout.");
                    }

                    Console.WriteLine("Replacement of value.");
                    updated_request = request2.Clone();
                    await token2.ReplaceValue(updated_request, token1.Value);
                    Console.WriteLine(await updated_request.Content.ReadAsStringAsync());

                    try
                    {
                        HttpResponseMessage response = await client.SendAsync(updated_request.GenerateRequest());
                        Console.WriteLine(response.StatusCode);
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
