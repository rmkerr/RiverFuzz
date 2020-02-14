using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HttpTokenize.Tokens;
using Newtonsoft.Json;

namespace HttpTokenize.Tokenizers
{
    public class BearerTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            TokenCollection tokens = new TokenCollection();

            if (request.Headers.ContainsKey("Authorization") && request.Headers["Authorization"].Count >= 1)
            {
                string bearerToken = request.Headers["Authorization"][0].Split(' ', 2)[1];
                tokens.Add(new BearerToken(bearerToken));
            }
            return tokens;
        }

        public TokenCollection ExtractTokens(Response response)
        {
            TokenCollection tokens = new TokenCollection();

            if (!response.Headers.ContainsKey("Content-Type") ||
                (response.Headers["Content-Type"].Count >= 1 &&
                 response.Headers["Content-Type"][0].Contains("json")))
            {
                try
                {
                    Regex rx = new Regex(@"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    JsonTextReader reader = new JsonTextReader(new StringReader(response.Content));
                    while (reader.Read())
                    {
                        if (reader.Value != null && reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                        {
                            reader.Read();
                            if (reader.TokenType == Newtonsoft.Json.JsonToken.String && rx.IsMatch(reader.Value.ToString()))
                            {
                                string? bearer_token = reader.Value.ToString();
                                if (bearer_token != null)
                                {
                                    tokens.Add(new BearerToken(bearer_token));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("JSON parsing failure.");
                }
            }

            return tokens;
        }
    }
}
