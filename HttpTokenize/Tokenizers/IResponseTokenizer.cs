using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;

namespace HttpTokenize.Tokenizers
{
    public interface IResponseTokenizer
    {
        public List<IToken> ExtractTokens(Response response);
    }
}
