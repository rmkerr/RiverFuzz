using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public class BearerTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public List<IToken> ExtractTokens(Request request)
        {
            List<IToken> tokens = new List<IToken>();
            try
            {
                if (request.Headers.ContainsKey("Authorization"))
                {
                    string bearerToken = request.Headers["Authorization"].Split(' ', 2)[1];
                    tokens.Add(new BearerToken(bearerToken));
                }
            }
            catch
            {
                // TODO: better error handling.
            }
            return tokens;
        }

        public List<IToken> ExtractTokens(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
