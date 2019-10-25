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

        public void Remove(Request request)
        {
            ReplacementHelper(request, string.Empty, true);
        }

        public void ReplaceName(Request request, string value)
        {
            throw new NotImplementedException();
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            ReplaceValue(request, replacement.Value);
        }

        public void ReplaceValue(Request request, string value)
        {
            ReplacementHelper(request, value, false);
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
