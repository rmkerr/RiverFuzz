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
        Boolean = 4
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

        public Task CopyIntoRequest(Request request);
        public Task ReplaceValue(Request request, string name);
        public Task ReplaceName(Request request, string value);
        public Task ReplaceToken(Request request, IToken replacement);
    }
}
