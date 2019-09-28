using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokenizers
{
    public interface ITokenizer
    {
        public List<IToken> ExtractTokens(Request request);
    }
}
