using HttpTokenize;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Generators
{
    public interface IGenerator
    {
        public IEnumerable<RequestSequence> Generate(List<RequestResponsePair> endpoints, RequestSequence sequence, List<TokenCollection> sequenceResults);
    }
}
