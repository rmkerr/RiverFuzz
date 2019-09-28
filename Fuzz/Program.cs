using System;
using HttpTokenize;
using HttpTokenize.Tokens;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fuzz
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Request request1 = new Request();
            request1.Url = new Uri("https://news.ycombinator.com/news?z=y&p=2&y=3&k=p");
            request1.Method = HttpMethod.Get;

            Request request2 = new Request();
            request2.Url = new Uri("https://news.ycombinator.com/user?id=maxrmk");

            HttpClient client = new HttpClient();
            foreach (IToken token1 in request1.GetTokens())
            {
                foreach (IToken token2 in request2.GetTokens())
                {
                    Console.WriteLine($"\nToken 1: {token1}");
                    Console.WriteLine($"Token 2: {token2}");

                    Console.WriteLine("Full replacement of name and value.");
                    Request updated_request = request2.Clone();
                    token2.ReplaceToken(updated_request, token1);
                    Console.WriteLine(updated_request.Url);
                    HttpResponseMessage response = await client.SendAsync(request1.GenerateRequest());
                    Console.WriteLine(response.StatusCode);

                    Console.WriteLine("Replacement of name.");
                    updated_request = request2.Clone();
                    token2.ReplaceName(updated_request, token1.Name);
                    Console.WriteLine(updated_request.Url);
                    response = await client.SendAsync(request1.GenerateRequest());
                    Console.WriteLine(response.StatusCode);

                    Console.WriteLine("Replacement of value.");
                    updated_request = request2.Clone();
                    token2.ReplaceValue(updated_request, token1.Value);
                    Console.WriteLine(updated_request.Url);
                    response = await client.SendAsync(request1.GenerateRequest());
                    Console.WriteLine(response.StatusCode);
                }
            }
        }
    }
}
