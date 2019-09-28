using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public class QueryTokenizer : ITokenizer
    {
        public List<IToken> ExtractTokens(string query)
        {
            List<IToken> tokens = new List<IToken>();
            NameValueCollection parameters = HttpUtility.ParseQueryString(query);

            foreach (string key in parameters.AllKeys)
            {
                tokens.Add(new QueryToken(key, parameters[key], Types.Integer | Types.String));
            }

            return tokens;
        }
    }
}
