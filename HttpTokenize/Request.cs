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
    public class RequestRequirement
    {
        public RequestRequirement(string name, Types supportedTypes)
        {
            Name = name;
            SupportedTypes = supportedTypes;
        }

        public string Name { get; }
        public Types SupportedTypes { get; }
    }

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

        public List<RequestRequirement> GetRequirements(List<IRequestTokenizer> tokenizers)
        {
            List<IToken> tokens = new List<IToken>();
            foreach (IRequestTokenizer tokenizer in tokenizers)
            {
                tokens.AddRange(tokenizer.ExtractTokens(this));
            }

            List<RequestRequirement> requirements = new List<RequestRequirement>();
            foreach (IToken token in tokens)
            {
                requirements.Add(new RequestRequirement(token.Name, token.SupportedTypes));
            }
            return requirements;
        }

        public Uri Url { get; set; }
        public HttpMethod Method { get; set; }
        public string Content { get; set; }

    }
}
