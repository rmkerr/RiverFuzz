using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize
{
    // Holds a request and a sequence of substitutions into that request.
    // Execution sequence will be 'perform substitutions' -> 'execute request'
    class Stage
    {
        List<ISubstitution> Substitutions;
        public Request? Request;


    }

    public interface ISubstitution
    {
        // TODO: Rework th
        public void MakeSubstitution(RequestSequence? previous, Request? next);

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
        public void MakeSubstitution(RequestSequence? previous, Request? next)
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

    public class RequestSequence
    {
    }
}
