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

        public void CopyIntoRequest(Request request)
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

        public void ReplaceValue(Request request, string value)
        {
            request.Headers["Authorization"] = value;
        }
    }
}
