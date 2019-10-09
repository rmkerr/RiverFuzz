using HttpTokenize.Substitutions;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpTokenize
{
    // Holds a request and a sequence of substitutions into that request.
    // Execution sequence will be 'perform substitutions' -> 'execute request'
    public class Stage
    {
        public List<ISubstitution> Substitutions { get; }
        public Request Request { get; }

        public Stage(Request request)
        {
            Request = request;
            Substitutions = new List<ISubstitution>();
        }

        public Stage Copy()
        {
            Stage stage = new Stage(Request);
            stage.Substitutions.AddRange(Substitutions);
            return stage;
        }
    }

    //
    public class RequestSequence : IEnumerable<Stage>
    {
        readonly List<Stage> Stages;
        private TokenCollection? Results;
        public RequestSequence()
        {
            Stages = new List<Stage>();
            Results = null;
        }

        public TokenCollection? GetResults()
        {
            return Results;
        }

        // TODO: More informative return information. Get rid of the stupid tuple.
        public async Task<List<Response>> Execute(HttpClient client, List<IResponseTokenizer> responseTokenizers, TokenCollection initialTokens)
        {
            TokenCollection tokens = new TokenCollection();
            List<Response> responses = new List<Response>();

            tokens.Add(initialTokens);

            // For each request.
            for (int i = 0; i < Stages.Count; ++i)
            {
                // Apply all substitutions.
                Request request = Stages[i].Request.Clone();
                foreach (ISubstitution substitution in Stages[i].Substitutions)
                {
                    substitution.MakeSubstitution(tokens, request);
                }

                // Make the request.
                try
                {
                    HttpResponseMessage rawResponse = await client.SendAsync(request.GenerateRequest());
                    Response response = new Response(rawResponse.StatusCode, await rawResponse.Content.ReadAsStringAsync());
                    if (rawResponse.Content.Headers.Contains("Content-Type"))
                    {
                        response.Headers.Add("Content-Type", rawResponse.Content.Headers.ContentType.ToString());
                    }
                    responses.Add(response);

                    // Parse the response and add tokens to the results.
                    TokenCollection results = response.GetResults(responseTokenizers);
                    tokens.Add(results);
                }
                catch
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, "//Timeout.");
                    responses.Add(response);
                }
            }
            Results = tokens;
            return responses;
        }

        public void Add(Stage stage)
        {
            Stages.Add(stage);
            Results = null;
        }

        public RequestSequence Copy()
        {
            RequestSequence sequence = new RequestSequence();
            foreach (Stage stage in Stages)
            {
                sequence.Add(stage.Copy());
            }
            if (Results != null)
            {
                sequence.Results = new TokenCollection(Results);
            }
            return sequence;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RequestSequence\n");
            foreach (Stage stage in Stages)
            {
                sb.Append("\t" + stage.Request.Url.ToString() + "\n");
                foreach (ISubstitution sub in stage.Substitutions)
                {
                    sb.Append("\t\t" + sub.ToString() + "\n");
                }
            }
            return sb.ToString();
        }

        public IEnumerator<Stage> GetEnumerator()
        {
            return ((IEnumerable<Stage>)Stages).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Stage>)Stages).GetEnumerator();
        }
    }
}
