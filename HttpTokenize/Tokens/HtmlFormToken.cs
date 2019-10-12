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
            string content = request.Content;
            StringBuilder sb = new StringBuilder(content.Length + value.Length);

            string[] pairs = content.Split('&');
            
            for(int i = 0; i < pairs.Length; ++i)
            {
                string[] data = pairs[i].Split('=', 2);
                
                if (data[0] == Name)
                {
                    sb.Append(data[0]);
                    sb.Append('=');
                    sb.Append(HttpUtility.UrlEncode(value));
                }
                else
                {
                    sb.Append(pairs[i]);
                }

                if (i < (pairs.Length - 1))
                {
                    sb.Append("&");
                }
            }

            request.Content = sb.ToString();
        }
    }
}
