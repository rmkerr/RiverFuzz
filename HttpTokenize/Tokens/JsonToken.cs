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
        public JsonToken(string name, string value, Types supported)
        {
            Name = name;
            Value = value;
            SupportedTypes = supported;
        }

        public string Name { get; }
        public string Value { get; }
        public Types SupportedTypes { get; }

        public void CopyIntoRequest(Request request)
        {
            string content = request.Content;
            try
            {
                JObject json_content = JObject.Parse(content);

                json_content.Add(Name, JValue.CreateString(Value));
                request.Content = json_content.ToString();
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public void ReplaceName(Request request, string name)
        {
            string content = request.Content;
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken($"..{Name}");
                JContainer parent = token.Parent;

                parent.Replace(new JProperty(name, token));

                request.Content = json_content.ToString();
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            string content = request.Content;
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken($"..{Name}");
                JContainer parent = token.Parent;

                if ((SupportedTypes & Types.Integer) != Types.None)
                {
                    parent.Replace(new JProperty(replacement.Name, int.Parse(replacement.Value)));
                }
                else
                {
                    parent.Replace(new JProperty(replacement.Name, JValue.CreateString(replacement.Value)));
                }

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
                JToken token = json_content.SelectToken($"..{Name}");
                token.Replace(JValue.CreateString(value));

                request.Content = json_content.ToString();
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public override string ToString()
        {
            return $"JsonToken: {Name}={Value}";
        }
    }
}
