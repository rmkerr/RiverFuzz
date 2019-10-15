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
            Uri url = request.Url;

            StringBuilder builder = new StringBuilder();
            builder.Append(url.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));

            string[] segments = url.AbsolutePath.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                builder.Append('/');
                if (i == Index)
                {
                    builder.Append(value);
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
