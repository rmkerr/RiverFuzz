using HttpTokenize.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HttpTokenize.Tokenizers
{
    public class JsonTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            if (!request.Headers.ContainsKey("Content-Type") || request.Headers["Content-Type"].Contains("json"))
            {
                return JsonToTokens(request.Content);
            }
            return new TokenCollection();
        }

        public TokenCollection ExtractTokens(Response response)
        {
            if (!response.Headers.ContainsKey("Content-Type") || response.Headers["Content-Type"].Contains("json"))
            {
                return JsonToTokens(response.Content);
            }
            return new TokenCollection();
        }

        private TokenCollection JsonToTokens(string json)
        {
            TokenCollection tokens = new TokenCollection();

            try
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(json));
                while (reader.Read())
                {
                    if (reader.Value != null && reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                    {
                        string name = reader.Value.ToString();
                        string path = reader.Path;
                        Console.WriteLine(path);
                        reader.Read();
                        if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
                        {
                            tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), path, Types.String));
                        }
                        else if (reader.TokenType == Newtonsoft.Json.JsonToken.Integer)
                        {
                            tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), path, Types.Integer));
                        }
                        else if (reader.TokenType == Newtonsoft.Json.JsonToken.Boolean)
                        {
                            tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), path, Types.Boolean));
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("JSON parsing failure.");
            }

            return tokens;
        }
    }
}
