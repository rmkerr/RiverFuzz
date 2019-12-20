using System;
using System.Collections.Generic;
using System.Linq;
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
            request.Headers["Authorization"][0] = "Bearer " + RemoveUnicode(value);
        }

        public void Remove(Request request)
        {
            request.Headers.Remove("Authorization");
        }

        private string RemoveUnicode(string value)
        {
            if (value.Any(c => c > 127))
            {
                Console.WriteLine("WARNING: Removing Non-ASCII characters from header.");
                return Encoding.ASCII.GetString(
                    Encoding.Convert(
                        Encoding.UTF8,
                        Encoding.GetEncoding(
                            Encoding.ASCII.EncodingName,
                            new EncoderReplacementFallback(string.Empty),
                            new DecoderExceptionFallback()
                            ),
                        Encoding.UTF8.GetBytes(value)
                    )
                );
            }
            return value;
        }

        // TODO: This just picks by type. We can do way better than this.
        public IToken? FindClosestEquivalent(TokenCollection tokens)
        {
            List<IToken> matches = tokens.GetByType(Types.BearerToken);
            if (matches.Count >= 1)
            {
                return matches[0];
            }
            return null;
        }
    }
}
