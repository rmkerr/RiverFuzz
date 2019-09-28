using HttpTokenize.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HttpTokenize.Tokenizers
{
    class JsonTokenizer
    {
        List<IToken> ExtractTokens(string json)
        {
            List<IToken> tokens = new List<IToken>();

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
