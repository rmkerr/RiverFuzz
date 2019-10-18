using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;

namespace HttpTokenize.Bucketers
{
    public class ExactStringBucketer : IBucketer
    {
        public ExactStringBucketer()
        {
        }

        private Dictionary<string, List<Response>> Sorted = new Dictionary<string, List<Response>>();

        public bool Add(Response response, TokenCollection tokens)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(response.Status.ToString());
            sb.Append(response.Content);

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
