using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;

namespace HttpTokenize
{
    public class Response
    {
        public Response(HttpStatusCode status, string content)
        {
            Status = status;
            Content = content;
            Headers = new Dictionary<string, string>();
        }

        public TokenCollection GetResults(List<IResponseTokenizer> tokenizers)
        {
            TokenCollection tokens = new TokenCollection();
            foreach (IResponseTokenizer tokenizer in tokenizers)
            {
                tokens.Add(tokenizer.ExtractTokens(this));
            }
            return tokens;
        }

        public HttpStatusCode Status { get; }
        public string Content { get; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
