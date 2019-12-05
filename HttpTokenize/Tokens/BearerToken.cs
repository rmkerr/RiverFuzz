using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpTokenize.Tokens
{
    public class BearerToken : IToken
    {
        public string Name { get; }
        public string Value { get; }
        public Types SupportedTypes { get; }

        public BearerToken(string token)
        {
            Name = "BearerToken";
            Value = token;
            SupportedTypes = Types.BearerToken;
        }

        public void ReplaceName(Request request, string value)
        {
            throw new NotImplementedException();
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceValue(Request request, string value)
        {
            // We're trusting that there should only ever be one Authorization header here.
            request.Headers["Authorization"][0] = "Bearer " + value;
        }

        public void Remove(Request request)
        {
            request.Headers.Remove("Authorization");
        }
    }
}
