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
            return JsonToTokens(request.Content);
        }

        public TokenCollection ExtractTokens(Response response)
        {
            return JsonToTokens(response.Content);
        }

        private TokenCollection JsonToTokens(string json)
        {
            TokenCollection tokens = new TokenCollection();

            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value != null && reader.TokenType == Newtonsoft.Json.JsonToken.PropertyName)
                {
                    string name = reader.Value.ToString();
                    reader.Read();
                    if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
                    {
                        tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), Types.String));
                    }
                    else if (reader.TokenType == Newtonsoft.Json.JsonToken.Integer)
                    {
                        tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), Types.Integer));
                    }
                    else if (reader.TokenType == Newtonsoft.Json.JsonToken.Boolean)
                    {
                        tokens.Add(new Tokens.JsonToken(name, reader.Value.ToString(), Types.Boolean));
                    }
                }
            }
            return tokens;
        }
    }
}
