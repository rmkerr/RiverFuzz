using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpTokenize.Tokens
{
    [Flags]
    public enum Types
    {
        None = 0,
        Integer = 1,
        String = 2,
        Boolean = 4,
        BearerToken = 8
    };

    // TODO
    [Flags]
    public enum Encodings
    {
        None = 0,
        Base64 = 1,
        Url = 2
    };

    public interface IToken
    {
        public string Name { get; }
        public string Value { get; }
        public Types SupportedTypes { get; }

        public void CopyIntoRequest(Request request);
        public void ReplaceValue(Request request, string value);
        public void ReplaceName(Request request, string name);
        public void ReplaceToken(Request request, IToken replacement);
    }
}
