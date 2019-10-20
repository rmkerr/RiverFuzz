using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.RequestSequence
{
    public class RequestResponsePair
    {
        public Request Request { get; }
        public Response Response { get; }

        public RequestResponsePair(Request request, Response response)
        {
            Request = request;
            Response = response;
            InputTokens = null;
            OutputTokens = null;
        }

        public void Tokenize(List<IRequestTokenizer> requestTokenizers, List<IResponseTokenizer> responseTokenizers)
        {
            InputTokens = Request?.GetRequirements(requestTokenizers);
            OutputTokens = Response?.Tokenize(responseTokenizers);
        }

        public TokenCollection? InputTokens { get; set; }
        public TokenCollection? OutputTokens { get; set; }
    }
}
