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
        private List<TokenCollection>? Results;
        public RequestSequence()
        {
            Stages = new List<Stage>();
            Results = null;
        }

        public List<TokenCollection> GetResults()
        {
            return Results;
        }

        // TODO: More informative return information. Get rid of the stupid tuple.
        public async Task<List<Response>> Execute(HttpClient client, List<IResponseTokenizer> responseTokenizers, TokenCollection initialTokens)
        {
            List<Response> responses = new List<Response>();
            Results = new List<TokenCollection>();

            Results.Add(initialTokens);

            // For each request.
            for (int i = 0; i < Stages.Count; ++i)
            {
                // Make the request.
                try
                {
                    // Apply all substitutions.
                    Request request = Stages[i].Request.Clone();
                    foreach (ISubstitution substitution in Stages[i].Substitutions)
                    {
                        substitution.MakeSubstitution(Results, request);
                    }

                    HttpResponseMessage rawResponse = await client.SendAsync(request.GenerateRequest());
                    Response response = new Response(rawResponse.StatusCode, await rawResponse.Content.ReadAsStringAsync());
                    if (rawResponse.Content.Headers.Contains("Content-Type"))
                    {
                        response.Headers.Add("Content-Type", rawResponse.Content.Headers.ContentType.ToString());
                    }
                    responses.Add(response);

                    // Parse the response and add tokens to the results.
                    Results.Add(response.GetResults(responseTokenizers));
                }
                catch (TimeoutException ex)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, "//Timeout.");
                    responses.Add(response);
                }
                catch (TaskCanceledException ex)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, "//Timeout.");
                    responses.Add(response);
                }
                catch (Exception ex)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, ex.Message);
                    responses.Add(response);
                }
            }

            return responses;
        }

        public void Add(Stage stage)
        {
            Stages.Add(stage);
            Results = null;
        }

        public Stage Get(int index)
        {
            return Stages[index];
        }

        public int Count()
        {
            return Stages.Count;
        }

        public RequestSequence Copy()
        {
            RequestSequence sequence = new RequestSequence();
            foreach (Stage stage in Stages)
            {
                sequence.Add(stage.Copy());
            }
            return sequence;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("RequestSequence\n");
            foreach (Stage stage in Stages)
            {
                foreach (ISubstitution sub in stage.Substitutions)
                {
                    sb.Append("\t\t" + sub.ToString() + "\n");
                }
                sb.Append("\t" + stage.Request.Method.ToString() + " " + stage.Request.Url.ToString() + "\n");
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
