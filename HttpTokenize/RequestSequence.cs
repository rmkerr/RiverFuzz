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
    }

    public interface ISubstitution
    {
        public void MakeSubstitution(TokenCollection previous, Request next);

        // TODO: Rework this. It's odd that we don't initialize with a token, but
        // we compare to one here.
        public bool ReplacesToken(IToken token);
    }

    public class SubstituteConstant : ISubstitution
    {
        private readonly IToken target;
        private readonly string value;
        public SubstituteConstant(IToken token, string constant)
        {
            target = token;
            value = constant;
        }
        public void MakeSubstitution(TokenCollection previous, Request next)
        {
            if (next != null)
            {
                target.ReplaceValue(next, value);
            }
        }

        public bool ReplacesToken(IToken token)
        {
            return token.GetType() == target.GetType() && token.Name == target.Name;
        }
    }

    public class SubstituteNamedToken : ISubstitution
    {
        private readonly IToken target;
        private readonly string sourceName;
        private readonly Types sourceType;
        public SubstituteNamedToken (IToken token, string name, Types type)
        {
            target = token;
            sourceName = name;
            sourceType = type;
        }
        public void MakeSubstitution(TokenCollection previous, Request next)
        {
            // Get token from previous sequence and replace new value with old one.
            IToken? replacement = previous.GetByName(sourceName);

            if (replacement == null)
            {
                throw new Exception($"Unable to find token by name '{sourceName}'.");
            }

            target.ReplaceValue(next, replacement.Value);
        }

        public bool ReplacesToken(IToken token)
        {
            return token.GetType() == target.GetType() && token.Name == target.Name;
        }
    }

    public class RequestSequence : IEnumerable<Stage>
    {
        readonly List<Stage> Stages;
        public RequestSequence()
        {
            Stages = new List<Stage>();
        }

        // TODO: More informative return information.
        public async Task<List<Response>?> Execute(HttpClient client, List<IResponseTokenizer> responseTokenizers)
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

                TokenCollection results = response.GetResults(responseTokenizers);
                tokens.Add(results);
            }
            return responses;
        }

        public void Add(Stage stage)
        {
            Stages.Add(stage);
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
