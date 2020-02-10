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
        public List<KnownEndpoint> Endpoints { get; set; }
        public List<RequestSequence> Population { get; internal set; }

        // Maps from each endpoint to all request sequences ending in that request.
        // private Dictionary<Request, List<RequestSequence>> AllKnownSequences;
        // Per-endpoint bucketer configuration.
        private Dictionary<Request, IBucketer> Bucketers;

        public PopulationManager()
        {
            Endpoints = new List<KnownEndpoint>();
            Population = new List<RequestSequence>();
            Bucketers = new Dictionary<Request, IBucketer>();
            // AllKnownSequences = new Dictionary<Request, List<RequestSequence>>();

            Population.Add(new RequestSequence());
        }

        public void AddEndpoint(KnownEndpoint endpoint, IBucketer bucketer)
        {
            Endpoints.Add(endpoint);
            Bucketers.Add(endpoint.Request.OriginalEndpoint, bucketer);
        }

        public void AddResponse(RequestSequence sequence)
        {
            Request finalRequest = sequence.Get(sequence.StageCount() - 1).Request;
            Bucketers[finalRequest.OriginalEndpoint].Add(sequence);
        }

        public void MinimizePopulation()
        {
            Population = new List<RequestSequence>();

            // We always need the option of starting from an empty sequence.
            Population.Add(new RequestSequence());

            foreach (IBucketer bucketer in Bucketers.Values)
            {
                // Get the bucketed requests, then reset the bucketer to free up memory.
                List<List<RequestSequence>> buckets = bucketer.Bucketize();
                bucketer.Reset();

                // Go through each bucket and pick the shortest sequence to save.
                foreach (List<RequestSequence> bucket in buckets)
                {
                    RequestSequence shortest = null;
                    foreach (RequestSequence candidate in bucket)
                    {
                        bool candidateIsValid = (candidate.GetResponses().Count > 0 && candidate.GetLastResponse().Status != HttpStatusCode.RequestTimeout);
                        if (candidateIsValid && (shortest == null ||
                            (candidate.StageCount() < shortest.StageCount() ||
                            (candidate.StageCount() == shortest.StageCount() &&
                            candidate.SubstitutionCount() < shortest.SubstitutionCount()))))
                        {
                            shortest = candidate;
                        }
                    }
                    if (shortest != null)
                    {
                        Population.Add(shortest);
                        bucketer.Add(shortest);
                    }
                }
            }

        }

        public string Summary()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KnownEndpoint endpoint in Endpoints)
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
