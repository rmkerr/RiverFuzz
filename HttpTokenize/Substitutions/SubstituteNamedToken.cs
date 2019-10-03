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
        public SubstituteNamedToken(IToken token, string name, Types type)
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

        public override string ToString()
        {
            return $"SubstituteNamedToken Target: {target.Name} Source: {sourceName}";
        }
    }
}
