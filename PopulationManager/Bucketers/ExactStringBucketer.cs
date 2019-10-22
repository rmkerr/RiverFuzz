using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize;
using HttpTokenize.Tokens;

namespace Population.Bucketers
{
    public class ExactStringBucketer : IBucketer
    {
        public ExactStringBucketer()
        {
        }

        private Dictionary<string, List<RequestSequence>> Sorted = new Dictionary<string, List<RequestSequence>>();

        public bool Add(RequestSequence sequence)
        {
            List<Response> responses = sequence.GetResponses();
            Response response = responses[responses.Count - 1];

            StringBuilder sb = new StringBuilder();
            sb.Append(response.Status.ToString());
            sb.Append(response.Content);

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
