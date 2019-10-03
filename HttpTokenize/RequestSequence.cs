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
        public RequestSequence()
        {
            Stages = new List<Stage>();
        }

        // TODO: More informative return information. Get rid of the stupid tuple.
        public async Task<Tuple<List<Response>, TokenCollection>> Execute(HttpClient client, List<IResponseTokenizer> responseTokenizers)
        {
            TokenCollection tokens = new TokenCollection();
            List<Response> responses = new List<Response>();

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
                HttpResponseMessage rawResponse = await client.SendAsync(request.GenerateRequest());
                Response response = new Response(rawResponse.StatusCode, await rawResponse.Content.ReadAsStringAsync());
                responses.Add(response);

                // Parse the response and add tokens to the results.
                TokenCollection results = response.GetResults(responseTokenizers);
                tokens.Add(results);
            }
            return new Tuple<List<Response>, TokenCollection>(responses, tokens);
        }

        public void Add(Stage stage)
        {
            Stages.Add(stage);
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
