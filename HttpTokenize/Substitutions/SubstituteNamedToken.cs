using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Substitutions
{
    public class SubstituteNamedToken : ISubstitution
    {
        private readonly IToken target;
        private readonly IToken source;
        private readonly Types sourceType;
        private readonly int sourceResponse;
        public SubstituteNamedToken(IToken target, IToken source, int responseIndex)
        {
            this.target = target;
            this.source = source;
            sourceResponse = responseIndex;
        }
        public IToken GetTarget()
        {
            return target;
        }

        public void MakeSubstitution(List<TokenCollection> previous, Request next)
        {
            // Get token from previous sequence and replace new value with old one.
            IToken? replacement = source.FindClosestEquivalent(previous[sourceResponse]);

            if (replacement == null)
            {
                throw new Exception($"Unable to find token '{source.Name}' '{source.ToString()}'.");
            }

            target.ReplaceValue(next, replacement.Value);
        }

        public bool ReplacesToken(IToken token)
        {
            return token.GetType() == target.GetType() && token.Name == target.Name;
        }

        public override string ToString()
        {
            if (sourceResponse == 0)
            {
                return $"Replace the value of '{target.Name}' with the token '{source.Name}' from seed tokens.";
            }
            return $"Replace the value of '{target.Name}' with the token '{source.Name}' from response #{sourceResponse}.";
        }
    }
}
