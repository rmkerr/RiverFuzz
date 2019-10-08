using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public class HtmlFormTokenizer : IRequestTokenizer, IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            TokenCollection tokens = new TokenCollection();
            if (request.Headers.ContainsKey("Content-Type") &&
                request.Headers["Content-Type"] == "application/x-www-form-urlencoded")
            {
                string[] pairs = request.Content.Split("&");
                foreach (string pair in pairs)
                {
                    if (pair.Length >= 3)
                    {
                        string[] data = pair.Split('=', 2);
                        HtmlFormToken token = new HtmlFormToken(HttpUtility.UrlDecode(data[0]), HttpUtility.UrlDecode(data[1]), TypeGuesser.GuessTypes(HttpUtility.UrlDecode(data[1])));
                        tokens.Add(token);
                    }
                }
            }
            return tokens;
        }

        public TokenCollection ExtractTokens(Response response)
        {
            throw new NotImplementedException();
        }
    }
}
