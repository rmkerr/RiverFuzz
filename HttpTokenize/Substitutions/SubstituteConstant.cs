using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Substitutions
{
    public class SubstituteConstant : ISubstitution
    {
        private readonly IToken target;
        private readonly string value;
        public SubstituteConstant(IToken token, string constant)
        {
            target = token;
            value = constant;
        }

        public IToken GetTarget()
        {
            return target;
        }

        public void MakeSubstitution(List<Response> previous, Request next)
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

        public override string ToString()
        {
            return $"SubstituteConstant Target: {target.Name} Value: {value}";
        }
    }
}
