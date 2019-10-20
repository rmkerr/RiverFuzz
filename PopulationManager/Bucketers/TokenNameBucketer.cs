using HttpTokenize;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace PopulationManager.Bucketers
{
    public class TokenNameBucketer : IBucketer
    {
        private Dictionary<string, List<Response>> Sorted = new Dictionary<string, List<Response>>();
        public TokenNameBucketer()
        {
        }

        public bool Add(Response response, TokenCollection tokens)
        {
            SortedSet<string> tokenNames = new SortedSet<string>();

            foreach (IToken token in tokens)
            {
                tokenNames.Add(token.Name);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(response.Status.ToString());
            foreach (string name in tokenNames)
            {
                sb.Append(name);
            }

            if (!Sorted.ContainsKey(sb.ToString()))
            {
                Sorted.Add(sb.ToString(), new List<Response>());
                Sorted[sb.ToString()].Add(response);
                return true;
            }
            Sorted[sb.ToString()].Add(response);
            return false;
        }

        public List<List<Response>> Bucketize()
        {
            List<List<Response>> ret = new List<List<Response>>();
            foreach (string key in Sorted.Keys)
            {
                ret.Add(Sorted[key]);
            }

            return ret;
        }
    }
}
