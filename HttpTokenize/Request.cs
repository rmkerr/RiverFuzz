using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using HttpTokenize.Tokens;
using Newtonsoft.Json;
using System.IO;
using HttpTokenize.Tokenizers;

namespace HttpTokenize
{
    public class Request
    {
        public Request()
        {
            Method = HttpMethod.Get;
            Headers = new Dictionary<string, string>();
        }

        public Request Clone()
        {
            Request clone = new Request();
            clone.Url = Url;
            clone.Method = Method;
            clone.Content = Content;
            clone.Headers = new Dictionary<string, string>(Headers);

            return clone;
        }

        public HttpRequestMessage GenerateRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(Method, Url);
            request.Content = new StringContent(Content);
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");

            //request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");

            return request;
        }

        public List<IToken> GetRequirements(List<IRequestTokenizer> tokenizers)
        {
            List<IToken> tokens = new List<IToken>();
            foreach (IRequestTokenizer tokenizer in tokenizers)
            {
                tokens.AddRange(tokenizer.ExtractTokens(this));
            }
            return tokens;
        }

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
