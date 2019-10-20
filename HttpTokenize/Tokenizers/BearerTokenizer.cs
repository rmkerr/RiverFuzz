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

            if (!request.Headers.ContainsKey("Content-Type") || request.Headers["Content-Type"].Contains("json"))
            {
                try
                {
                    if (request.Headers.ContainsKey("Authorization"))
                    {
                        string bearerToken = request.Headers["Authorization"].Split(' ', 2)[1];
                        tokens.Add(new BearerToken(bearerToken));
                    }
                }
                catch
                {
                }
            }
            return tokens;
        }

        public TokenCollection ExtractTokens(Response response)
        {
            TokenCollection tokens = new TokenCollection();

            if (!response.Headers.ContainsKey("Content-Type") || response.Headers["Content-Type"].Contains("json"))
            {
                try
                {
                    Regex rx = new Regex(@"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    JsonTextReader reader = new JsonTextReader(new StringReader(response.Content));
                    while (reader.Read())
                    {
                        if (reader.Value != null && reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                        {
                            string name = reader.Value.ToString();
                            reader.Read();
                            if (reader.TokenType == Newtonsoft.Json.JsonToken.String && rx.IsMatch(reader.Value.ToString()))
                            {
                                tokens.Add(new BearerToken(reader.Value.ToString()));
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
