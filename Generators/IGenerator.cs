using HttpTokenize;
using HttpTokenize.RequestSequence;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generators
{
    public interface IGenerator
    {
        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, TokenCollection initialTokens);
    }
}
