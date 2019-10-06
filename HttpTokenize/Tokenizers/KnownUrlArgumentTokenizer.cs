using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public class KnownUrlArgumentTokenizer : IRequestTokenizer
    {
        public TokenCollection ExtractTokens(Request request)
        {
            TokenCollection tokens = new TokenCollection();


            string[] segments = request.Url.AbsolutePath.Split('/');
            for (int i = 0; i < segments.Length; ++i)
            {
                string segment = HttpUtility.UrlDecode(segments[i]);
                if (segment.Length >= 3 && segment[0] == '{' && segment[segment.Length - 1] == '}')
                {
                    string trimmed = segment.Trim('{', '}');
                    string[] parsed = trimmed.Split(':');

                    // TODO: Parse supported types.
                    tokens.Add(new PathToken(i, parsed[0], parsed[1], Types.Integer));
                }
            }

            return tokens;
        }
    }
}
