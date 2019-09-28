using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using HttpTokenize.Tokens;
using Newtonsoft.Json;
using System.IO;

namespace HttpTokenize
{
    public class Request
    {
        public Request()
        {
            Method = HttpMethod.Get;
        }

        public Request Clone()
        {
            Request clone = new Request();
            clone.Url = Url;
            clone.Method = Method;

            return clone;
        }

        public HttpRequestMessage GenerateRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(Method, Url);
            return request;
        }

        public List<IToken> GetTokens()
        {
            List<IToken> tokens = new List<IToken>();
            NameValueCollection parameters = HttpUtility.ParseQueryString(Url.Query);

            foreach (string key in parameters.AllKeys)
            {
                tokens.Add(new QueryToken(key, parameters[key], Types.Integer | Types.String));
            }

            // TODO: iff json
            string body = Content.ReadAsStringAsync().Result;
            JsonTextReader reader = new JsonTextReader(new StringReader(body));

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

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public HttpContent Content { get; set; }

    }
}
