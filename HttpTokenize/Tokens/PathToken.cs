using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    public class PathToken : IToken
    {
        private int Index;

        // Replace the nth segment in a path.
        public PathToken(int index, string name, string value, Types supportedTypes)
        {
            Index = index;
            SupportedTypes = supportedTypes;
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public Types SupportedTypes { get; }

        public void DeleteToken(Request request)
        {
            ReplacementHelper(request, string.Empty, true);
        }

        public void ReplaceValue(Request request, string value)
        {
            ReplacementHelper(request, value, false);
        }

        public IToken? FindClosestEquivalent(TokenCollection tokens)
        {
            List<IToken> matches = tokens.GetByName(this.Name);
            if (matches.Count > 1)
            {
                Console.WriteLine("WARNING: Multiple cookies with the same name. This probably shouldn't happen.");
            }
            if (matches.Count >= 1)
            {
                return matches[0];
            }
            return null;
        }

        private void ReplacementHelper(Request request, string value, bool remove)
        {
            Uri url = request.Url;

            StringBuilder builder = new StringBuilder();
            builder.Append(url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));

            string[] segments = url.AbsolutePath.Trim('/').Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                builder.Append('/');
                if (i == Index)
                {
                    if (remove)
                    {
                        builder.Length--;
                    }
                    else
                    {
                        builder.Append(value);
                    }
                }
                else
                {
                    builder.Append(segments[i]);
                }
            }

            builder.Append(url.GetComponents(UriComponents.Query, UriFormat.UriEscaped));
            request.Url = new Uri(builder.ToString());
        }
    }
}
