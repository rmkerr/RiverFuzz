using HttpTokenize.Substitutions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.RequestSequence
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
}
