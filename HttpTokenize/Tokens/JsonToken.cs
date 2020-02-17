using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace HttpTokenize.Tokens
{
    public class JsonToken : IToken
    {
        public JsonToken(string name, string value, string path, Types supported)
        {
            Name = name;
            Value = value;
            Path = path;
            SupportedTypes = supported;
        }

        public string Name { get; }
        public string Value { get; }
        public string Path { get; }
        public Types SupportedTypes { get; }

        public void DeleteToken(Request request)
        {
            string content = request.Content;
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken(Path);
                token.Parent.Remove();
                //token.Remove();

                request.Content = json_content.ToString();
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public void ReplaceValue(Request request, string value)
        {
            string content = request.Content;
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken(Path);
                token.Replace(JValue.CreateString(value));

                request.Content = json_content.ToString();
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public IToken? FindClosestEquivalent(TokenCollection tokens)
        {
            List<IToken> matches = new List<IToken>();

            // Try to find exact match, but capture potential non-exact matches along the way.
            foreach (IToken potentialMatch in tokens)
            {
                JsonToken? jsonPotentialMatch = potentialMatch as JsonToken;
                if (jsonPotentialMatch != null)
                {
                    if (jsonPotentialMatch.Path == this.Path)
                    {
                        // Found an 'exact' match.
                        return jsonPotentialMatch;
                    }
                    else
                    {
                        matches.Add(jsonPotentialMatch);
                    }
                }
            }

            // We didn't find an exact match, but we may have found name matches.
            if (matches.Count > 1)
            {
                Console.WriteLine("WARNING: No exact JSON match, but many name matched. May need to improve hueristics.");
            }
            if (matches.Count >= 1)
            {
                return matches[0];
            }

            // No matches. We may have a reproduceability issue.
            return null;
        }

        public override string ToString()
        {
            return $"JsonToken: {Name}={Value}";
        }
    }
}
