using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Bucketers
{
    public class TokenNameBucketer : IBucketer
    {
        public TokenNameBucketer()
        {
            Responses = new List<Response>();
        }
        public List<Response> Responses { get; }

        public List<List<Response>> Bucketize()
        {
            // TODO: Make this less terrible/more performant.
            // Currently it takes the names of all the parameters,
            // turns them into a big string and hashes that string...

            Dictionary<string, List<Response>> sorted = new Dictionary<string, List<Response>>();
            JsonTokenizer tokenizer = new JsonTokenizer();

            foreach (Response response in Responses)
            {
                TokenCollection collection = tokenizer.ExtractTokens(response);
                StringBuilder sb = new StringBuilder();
                foreach (IToken token in collection)
                {
                    sb.Append(token.Name);
                }

                if (!sorted.ContainsKey(sb.ToString()))
                {
                    sorted.Add(sb.ToString(), new List<Response>());
                }
                sorted[sb.ToString()].Add(response);
            }

            List<List<Response>> ret = new List<List<Response>>();
            foreach (string key in sorted.Keys)
            {
                ret.Add(sorted[key]);
            }

            return ret;
        }
    }
}
