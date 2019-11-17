using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Substitutions
{
    public class SubstituteNamedToken : ISubstitution
    {
        private readonly IToken target;
        private readonly string sourceName;
        private readonly Types sourceType;
        private readonly int sourceResponse;
        public SubstituteNamedToken(IToken token, string name, int responseIndex, Types type)
        {
            target = token;
            sourceName = name;
            sourceType = type;
            sourceResponse = responseIndex;
        }
        public IToken GetTarget()
        {
            return target;
        }

        public void MakeSubstitution(List<TokenCollection> previous, Request next)
        {
            // Get token from previous sequence and replace new value with old one.
            List<IToken> replacement = previous[sourceResponse].GetByName(sourceName);

            if (replacement.Count == 0)
            {
                throw new Exception($"Unable to find token by name '{sourceName}'.");
            }

            target.ReplaceValue(next, replacement[0].Value);
        }

        public bool ReplacesToken(IToken token)
        {
            return token.GetType() == target.GetType() && token.Name == target.Name;
        }

        public override string ToString()
        {
            return $"Replace the value of '{target.Name}' with the value of '{sourceName}' from response '{sourceResponse}'.";
        }
    }
}
