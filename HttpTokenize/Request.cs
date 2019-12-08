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
            Headers = new Dictionary<string, List<string>>();
            original = this;
        }

        public Request Clone()
        {
            Request clone = new Request(Url, Method, Content);
            clone.Headers = new Dictionary<string, List<string>>();
            foreach (string headerName in Headers.Keys)
            {
                clone.Headers.Add(headerName, new List<string>(Headers[headerName]));
            }
            clone.original = original;

            return clone;
        }

        public HttpRequestMessage GenerateRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(Method, Url);
            request.Content = new StringContent(Content);
            if (Headers.ContainsKey("Content-Type") && Headers["Content-Type"].Count >= 1)
            {
                // If there are multiple content-type headers, grab only the first. I think this is safe?
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(Headers["Content-Type"][0]);
            }

            if (Headers.ContainsKey("Authorization") && Headers["Authorization"].Count >= 1)
            {
                // If there are multiple authorization headers, grab only the first. I'm pretty sure this is safe.
                string[] values = Headers["Authorization"][0].Split(' ');
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
        public Dictionary<string, List<string>> Headers { get; set; }
        public Request OriginalEndpoint { get { return original; } }

        // Used to track the source endpoint that this request is a 
        // variation of. Useful when bucketing.
        internal Request original;

        // ID that uniquely identifies this request. Primarily used by database.
        public int? Id { get; set; }
    }
}
