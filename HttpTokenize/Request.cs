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
            clone.Content = Content;

            return clone;
        }

        public HttpRequestMessage GenerateRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(Method, Url);
            request.Content = new StringContent(Content);
            request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
            return request;
        }

        public List<IToken> GetTokens()
        {
            
        }

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public string Content { get; set; }

    }
}
