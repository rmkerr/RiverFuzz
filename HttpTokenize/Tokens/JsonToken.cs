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

        public async Task CopyIntoRequest(Request request)
        {
            string content = await request.Content.ReadAsStringAsync();
            try
            {
                JObject json_content = JObject.Parse(content);

                json_content.Add(Name, JValue.CreateString(Value));
                request.Content = new StringContent(json_content.ToString());
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public async Task ReplaceName(Request request, string value)
        {
            string content = await request.Content.ReadAsStringAsync();
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken($"..{Name}");
                JContainer parent = token.Parent;

                parent.Replace(new JProperty(value, token));

                request.Content = new StringContent(json_content.ToString());
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public async Task ReplaceToken(Request request, IToken replacement)
        {
            string content = await request.Content.ReadAsStringAsync();
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

                request.Content = new StringContent(json_content.ToString());
            }
            catch
            {
                // TODO: Not JSON
            }
        }

        public async Task ReplaceValue(Request request, string value)
        {
            string content = await request.Content.ReadAsStringAsync();
            try
            {
                JObject json_content = JObject.Parse(content);
                JToken token = json_content.SelectToken($"..{Name}");
                token.Replace(JValue.CreateString(value));

                request.Content = new StringContent(json_content.ToString());
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
