using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Text;

namespace Population.Bucketers
{
    public class StatusCodeBucketer : IBucketer
    {
        private Dictionary<string, List<RequestSequence>> Sorted = new Dictionary<string, List<RequestSequence>>();

        public bool Add(RequestSequence sequence)
        {
            List<Response> responses = sequence.GetResponses();
            Response response = responses[responses.Count - 1];

            StringBuilder sb = new StringBuilder();
            sb.Append(response.Status.ToString());

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
