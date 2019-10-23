using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HttpTokenize;
using HttpTokenize.Tokens;
using Population.Bucketers;

namespace Population
{
    public class PopulationManager
    {
        public List<RequestResponsePair> Endpoints { get; set; }
        public List<RequestSequence> Population { get; internal set; }

        // Maps from each endpoint to all request sequences ending in that request.
        // private Dictionary<Request, List<RequestSequence>> AllKnownSequences;
        // Per-endpoint bucketer configuration.
        private Dictionary<Request, IBucketer> Bucketers;

        public PopulationManager()
        {
            Endpoints = new List<RequestResponsePair>();
            Population = new List<RequestSequence>();
            Bucketers = new Dictionary<Request, IBucketer>();
            // AllKnownSequences = new Dictionary<Request, List<RequestSequence>>();

            Population.Add(new RequestSequence());
        }

        public void AddEndpoint(RequestResponsePair endpoint, IBucketer bucketer)
        {
            Endpoints.Add(endpoint);
            Bucketers.Add(endpoint.Request.OriginalEndpoint, bucketer);
        }

        public void AddResponse(RequestSequence sequence)
        {
            Request finalRequest = sequence.Get(sequence.Count() - 1).Request;
            Bucketers[finalRequest.OriginalEndpoint].Add(sequence);

        }

        public void MinimizePopulation()
        {
            Population = new List<RequestSequence>();

            // We always need the option of starting from an empty sequence.
            Population.Add(new RequestSequence());

            foreach (IBucketer bucketer in Bucketers.Values)
            {
                foreach (List<RequestSequence> bucket in bucketer.Bucketize())
                {
                    if (bucket.Count > 0)
                    {
                        Response response = bucket[0].GetLastResponse();
                        if (response.Status != HttpStatusCode.RequestTimeout)
                        {
                            RequestSequence shortest = bucket[0];
                            foreach (RequestSequence candidate in bucket)
                            {
                                if (candidate.Count() < shortest.Count())
                                {
                                    shortest = candidate;
                                }
                            }
                            Population.Add(shortest);
                        }
                    }
                }
            }
        
        }

        public string Summary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (RequestResponsePair endpoint in Endpoints)
            {
                IBucketer bucketer = Bucketers[endpoint.Request.OriginalEndpoint];
                List<List<RequestSequence>> bucketed = bucketer.Bucketize();

                sb.AppendLine($"\nEndpoint {endpoint.Request.ToString()}");
                sb.AppendLine($"\t{bucketed.Count} buckets.");
                foreach (List<RequestSequence> bucket in bucketed)
                {
                    sb.AppendLine($"\t{bucket[0]}");
                    sb.AppendLine($"\t{bucket[0].GetLastResponse().ToString()}\n");
                }
            }
            return sb.ToString();
        }
    }
}
