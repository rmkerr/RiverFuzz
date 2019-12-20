using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    public class ConstantValueToken : IToken
    {
        public ConstantValueToken(string value, Types supported)
        {
            Name = $"ConstValue({value})";
            Value = value;
            SupportedTypes = supported;
        }
        public string Name { get; }

        public string Value { get; }

        public Types SupportedTypes { get; }

        // TODO: This may actually need an implementation.
        public IToken? FindClosestEquivalent(TokenCollection tokens)
        {
            throw new NotImplementedException();
        }

        public void Remove(Request request)
        {
            throw new NotImplementedException();
        }

        public void ReplaceName(Request request, string value)
        {
            throw new NotImplementedException();
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceValue(Request request, string name)
        {
            throw new NotImplementedException();
        }
    }
}
