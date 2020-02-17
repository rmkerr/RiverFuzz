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

        public void DeleteToken(Request request)
        {
            ReplaceHelper(request, string.Empty, true);
        }

        public void ReplaceValue(Request request, string value)
        {
            ReplaceHelper(request, value, false);
        }

        public IToken? FindClosestEquivalent(TokenCollection tokens)
        {
            List<IToken> matches = new List<IToken>();
            foreach (IToken potentialMatch in tokens)
            {
                if (potentialMatch.Name == this.Name && potentialMatch.GetType() == this.GetType())
                {
                    matches.Add(potentialMatch);
                }
            }
            if (matches.Count > 1)
            {
                Console.WriteLine("WARNING: Multiple form tokens with the same name. This probably shouldn't happen.");
                foreach (IToken match in matches)
                {
                    Console.WriteLine($"\t {match.ToString()}");
                }
            }
            if (matches.Count >= 1)
            {
                return matches[0];
            }
            return null;
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
