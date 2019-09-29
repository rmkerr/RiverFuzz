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

        public List<IToken> GetResults(List<IResponseTokenizer> tokenizers)
        {
            List<IToken> tokens = new List<IToken>();
            foreach (IResponseTokenizer tokenizer in tokenizers)
            {
                tokens.AddRange(tokenizer.ExtractTokens(this));
            }
            return tokens;
        }

        public HttpStatusCode Status { get; }
        public string Content { get; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
