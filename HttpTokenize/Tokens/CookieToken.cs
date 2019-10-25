using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Tokens
{
    public class CookieToken : IToken
    {
        public string Name { get; }

        public string Value { get; }

        public Types SupportedTypes { get; }

        public CookieToken(string name, string value, Types supported)
        {
            Name = name;
            Value = value;
            SupportedTypes = supported;
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
            string cookieHeader = request.Headers["Cookie"];
            string[] cookiePairs = cookieHeader.Split(';');

            StringBuilder sb = new StringBuilder(cookieHeader.Length + value.Length);
            for (int i = 0; i < cookiePairs.Length; ++i)
            {
                if (cookiePairs[i].StartsWith(Name + "="))
                {
                    sb.Append($"{Name}={value}");
                }
                else
                {
                    sb.Append(cookiePairs[i]);
                }
                sb.Append(";");
            }

            // Drop trailing semicolon.
            if (sb.Length > 0)
            {
                sb.Length--;
            }
            request.Headers["Cookie"] = sb.ToString();
        }

        public void Remove(Request request)
        {
            string cookieHeader = request.Headers["Cookie"];
            string[] cookiePairs = cookieHeader.Split(';');

            StringBuilder sb = new StringBuilder(cookieHeader.Length);
            for (int i = 0; i < cookiePairs.Length; ++i)
            {
                if (!cookiePairs[i].StartsWith(Name + "="))
                {
                    sb.Append(cookiePairs[i]);
                    sb.Append(";");
                }
            }

            // Drop trailing semicolon.
            if (sb.Length > 0)
            {
                sb.Length--;
            }
            request.Headers["Cookie"] = sb.ToString();
        }
    }
}
