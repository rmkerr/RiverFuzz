using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public interface IResponseTokenizer
    {
        public TokenCollection ExtractTokens(Response response);
    }
}
