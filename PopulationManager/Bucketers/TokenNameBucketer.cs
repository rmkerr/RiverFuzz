using HttpTokenize;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Population.Bucketers
{
    public class TokenNameBucketer : IBucketer
    {
        private Dictionary<string, List<RequestSequence>> Sorted = new Dictionary<string, List<RequestSequence>>();
        private List<string> UseValueOverrides;

        // Overrides allow us to 
        public TokenNameBucketer(string[] useValueOverrides = null)
        {
            if (useValueOverrides != null)
            {
                UseValueOverrides = new List<string>(useValueOverrides);
            }
            else
            {
                UseValueOverrides = new List<string>();
            }
        }

        public bool Add(RequestSequence sequence)
        {
            List<Response> responses = sequence.GetResponses();
            Response response = responses[responses.Count - 1];

            List<TokenCollection> allResults = sequence.GetResults();
            TokenCollection results = allResults[allResults.Count - 1];

            SortedSet<string> tokenNames = new SortedSet<string>();

            foreach (IToken token in results)
            {
                
                if (UseValueOverrides.Contains(token.Name))
                {
                    tokenNames.Add($"{token.Name}:{token.Value}");
                }
                else
                {
                    tokenNames.Add(token.Name);
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(response.Status.ToString());

            if (response.Status != System.Net.HttpStatusCode.RequestTimeout)
            {
                foreach (string name in tokenNames)
                {
                    sb.Append(name);
                }
            }

            if (!Sorted.ContainsKey(sb.ToString()))
            {
                Sorted.Add(sb.ToString(), new List<RequestSequence>());
                Sorted[sb.ToString()].Add(sequence);
                return true;
            }

            Sorted[sb.ToString()].Add(sequence);
            return false;
        }

        public List<List<RequestSequence>> Bucketize()
        {
            List<List<RequestSequence>> ret = new List<List<RequestSequence>>();
            foreach (string key in Sorted.Keys)
            {
                ret.Add(Sorted[key]);
            }

            return ret;
        }
    }
}
