using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using HttpTokenize.Tokens;
using System.Threading.Tasks;

namespace HttpTokenize
{
    public class QueryToken : IToken
    {
        public QueryToken(string name, string value, Types supported)
        {
            Name = name;
            Value = value;
            SupportedTypes = supported;
        }

        public string Name { get; }
        public string Value { get; }
        public Types SupportedTypes { get; }

        public void ReplaceValue(Request request, string value)
        {
            QueryToken token = new QueryToken(Name, value, Types.Integer | Types.String);
            ReplaceToken(request, token);
        }

        public void ReplaceName(Request request, string name)
        {
            QueryToken token = new QueryToken(name, Value, Types.Integer | Types.String);
            ReplaceToken(request, token);
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            Uri url = request.Url;
            NameValueCollection parameters = HttpUtility.ParseQueryString(url.Query);
            parameters[Name] = Value;

            StringBuilder builder = new StringBuilder();
            builder.Append(url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
            builder.Append("?");

            string[] keys = parameters.AllKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == Name)
                {
                    builder.Append(replacement.Name);
                    builder.Append("=");
                    builder.Append(replacement.Value);
                }
                else
                {
                    builder.Append(keys[i]);
                    builder.Append("=");
                    builder.Append(parameters[keys[i]]);
                }
                if (i < keys.Length - 1)
                {
                    builder.Append("&");
                }
            }

            request.Url = new Uri(builder.ToString());
        }

        public override string ToString()
        {
            return $"QueryToken: {Name}={Value}";
        }

        public void Remove(Request request)
        {
            Uri url = request.Url;
            NameValueCollection parameters = HttpUtility.ParseQueryString(url.Query);
            parameters[Name] = Value;

            StringBuilder builder = new StringBuilder();
            builder.Append(url.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.UriEscaped));
            builder.Append("?");

            string[] keys = parameters.AllKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] != Name)
                {
                    builder.Append(keys[i]);
                    builder.Append("=");
                    builder.Append(parameters[keys[i]]);
                    if (i < keys.Length - 1)
                    {
                        builder.Append("&");
                    }
                }
            }
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
    }
}
