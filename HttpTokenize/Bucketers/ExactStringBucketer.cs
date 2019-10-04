using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Bucketers
{
/*    public class ExactStringBucketer : IBucketer
    {
        public ExactStringBucketer()
        {
            Responses = new List<Response>();
        }
        public List<Response> Responses { get; }

        public List<List<Response>> Bucketize()
        {
            Dictionary<string, List<Response>> sorted = new Dictionary<string, List<Response>>();
            foreach (Response response in Responses)
            {
                if (!sorted.ContainsKey(response.Content))
                {
                    sorted.Add(response.Content, new List<Response>());
                }
                sorted[response.Content].Add(response);
            }

            List<List<Response>> ret = new List<List<Response>>();
            foreach (string key in sorted.Keys)
            {
                ret.Add(sorted[key]);
            }

            return ret;
        }
    }*/
}
