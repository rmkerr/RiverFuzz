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
        private List<Response>? Responses;

        public RequestSequence()
        {
            Stages = new List<Stage>();
            Results = null;
            Responses = null;
        }

        public List<TokenCollection>? GetResults()
        {
            return Results;
        }

        public TokenCollection? GetLastResult()
        {
            if (Results == null || Results.Count == 0)
            {
                return null;
            }
            return Results[Results.Count - 1];
        }

        public Response? GetLastResponse()
        {
            if (Responses == null || Responses.Count == 0)
            {
                return null;
            }
            return Responses[Responses.Count - 1];
        }

        public List<Response>? GetResponses()
        {
            return Responses;
        }

        // TODO: More informative return information. Get rid of the stupid tuple.
        public async Task<List<Response>> Execute(HttpClient client, List<IResponseTokenizer> responseTokenizers, TokenCollection initialTokens)
        {
            Responses = new List<Response>();
            Results = new List<TokenCollection>();

            Results.Add(initialTokens);

            // For each request.
            for (int i = 0; i < Stages.Count; ++i)
            {
                // Make the request.
                try
                {
                    // Apply all substitutions.
                    Request request = Stages[i].Request;
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
                    Responses.Add(response);

                    // Parse the response and add tokens to the results.
                    Results.Add(response.GetResults(responseTokenizers));
                }
                catch (TimeoutException)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, "//Timeout.");
                    Responses.Add(response);
                    break;
                }
                catch (TaskCanceledException)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, "//Timeout.");
                    Responses.Add(response);
                    break;
                }
                catch (Exception ex)
                {
                    Response response = new Response(System.Net.HttpStatusCode.RequestTimeout, ex.Message);
                    Responses.Add(response);
                    break;
                }
            }

            return Responses;
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

        public int StageCount()
        {
            return Stages.Count;
        }

        public int SubstitutionCount()
        {
            int subs = 0;
            foreach (Stage stage in Stages)
            {
                subs += stage.Substitutions.Count;
            }
            return subs;
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
