using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using HttpTokenize.Tokens;

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
               foreach

            return tokens;
        }

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public HttpContent Content { get; set; }

    }
}
