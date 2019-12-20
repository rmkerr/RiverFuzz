using System;
using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < request.Headers["Cookie"].Count; ++i)
            {
                string cookieHeader = request.Headers["Cookie"][i];
                string[] cookiePairs = cookieHeader.Split(';');

                StringBuilder sb = new StringBuilder(cookieHeader.Length + value.Length);
                for (int t = 0; t < cookiePairs.Length; ++t)
                {
                    if (cookiePairs[t].StartsWith(Name + "="))
                    {
                        sb.Append($"{Name}={RemoveUnicode(value)}");
                    }
                    else
                    {
                        sb.Append(cookiePairs[t]);
                    }
                    sb.Append(";");
                }

                // Drop trailing semicolon.
                if (sb.Length > 0)
                {
                    sb.Length--;
                }
                request.Headers["Cookie"][i] = sb.ToString();
            }
        }

        public void Remove(Request request)
        {
            string cookieHeader = request.Headers["Cookie"][0];
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
            request.Headers["Cookie"][0] = sb.ToString();
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
    }
}
