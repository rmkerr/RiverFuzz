using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokenizers
{
    public interface IRequestTokenizer
    {
        public TokenCollection ExtractTokens(Request request);
    }
}
