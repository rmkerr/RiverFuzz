using System;
using System.Net.Http;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using HttpTokenize.Tokens;
using Newtonsoft.Json;
using System.IO;
using HttpTokenize.Tokenizers;
using System.Net.Http.Headers;

namespace HttpTokenize
{
    public class Request
    {
        public Request(Uri url, HttpMethod method, string? content = null)
        {
            Method = method;
            Url = url;
            Content = content;
            Headers = new Dictionary<string, string>();
            original = this;
        }

        public Request Clone()
        {
            Request clone = new Request(Url, Method, Content);
            clone.Headers = new Dictionary<string, string>(Headers);
            clone.original = original;

            return clone;
        }

        public HttpRequestMessage GenerateRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(Method, Url);
            request.Content = new StringContent(Content);
            if (Headers.ContainsKey("Content-Type"))
            {
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(Headers["Content-Type"]);
            }

            if (Headers.ContainsKey("Authorization"))
            {
                string[] values = Headers["Authorization"].Split(' ');
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(values[0], values[1]);
            }

            if (Headers.ContainsKey("Cookie"))
            {
                request.Headers.Add("Cookie", Headers["Cookie"]);
            }

            return request;
        }

        public TokenCollection GetRequirements(List<IRequestTokenizer> tokenizers)
        {
            TokenCollection tokens = new TokenCollection();
            foreach (IRequestTokenizer tokenizer in tokenizers)
            {
                tokens.Add(tokenizer.ExtractTokens(this));
            }
            return tokens;
        }

        public override string ToString()
        {
            return Method.ToString() + " " + Url.ToString();
        }

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public string? Content { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Request OriginalEndpoint { get { return original; } }

        // Used to track the source endpoint that this request is a 
        // variation of. Useful when bucketing.
        internal Request original;
    }
}
