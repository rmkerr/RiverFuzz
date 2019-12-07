using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public class CookieTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            TokenCollection tokens = new TokenCollection();

            if (request.Headers.ContainsKey("Cookie"))
            {
                foreach (string cookieHeader in request.Headers["Cookie"])
                {
                    string[] cookies = cookieHeader.Split(';');

                    foreach (string cookieString in cookies)
                    {
                        string[] vals = cookieString.Split('=');
                        tokens.Add(new CookieToken(vals[0], vals[1], TypeGuesser.GuessTypes(vals[1])));
                    }
                }
            }

            return tokens;
        }

        public TokenCollection ExtractTokens(Response response)
        {
            TokenCollection tokens = new TokenCollection();

            if (response.Headers.ContainsKey("Set-Cookie"))
            {
                foreach (string cookieHeader in response.Headers["Set-Cookie"])
                {
                    string[] vals = cookieHeader.Split(';', '=');
                    tokens.Add(new CookieToken(vals[0], vals[1], TypeGuesser.GuessTypes(vals[1])));
                }
            }

            return tokens;
        }

    }
}
