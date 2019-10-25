using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace HttpTokenize.Tokens
{
    public class HtmlFormToken : IToken
    {
        public HtmlFormToken(string name, string value, Types supported)
        {
            Name = name;
            Value = value;
            SupportedTypes = supported;
        }

        public string Name { get; }

        public string Value { get; }

        public Types SupportedTypes { get; }

        public void Remove(Request request)
        {
            ReplaceHelper(request, string.Empty, true);
        }

        public void ReplaceName(Request request, string name)
        {
            throw new NotImplementedException();
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceValue(Request request, string value)
        {
            ReplaceHelper(request, value, false);
        }

        private void ReplaceHelper(Request request, string value, bool remove)
        {
            string content = request.Content;
            StringBuilder sb = new StringBuilder(content.Length + value.Length);

            string[] pairs = content.Split('&');

            for (int i = 0; i < pairs.Length; ++i)
            {
                string[] data = pairs[i].Split('=', 2);

                if (data[0] == Name && !remove)
                {
                    sb.Append(data[0]);
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode(value));
                }
                else
                {
                    sb.Append(pairs[i]);
                }
                sb.Append("&");
            }

            if (sb.Length > 0)
            {
                sb.Length--;
            }

            request.Content = sb.ToString();
        }
    }
}
