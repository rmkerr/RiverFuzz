using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace HttpTokenize.Tokens
{
    class JsonToken : IToken
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
            JObject json_content = JObject.Parse(content);
        }

        public void ReplaceName(Request request, string value)
        {
            throw new NotImplementedException();
        }

        public void ReplaceToken(Request request, IToken replacement)
        {
            throw new NotImplementedException();
        }

        public void ReplaceValue(Request request, string name)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"JsonToken: {Name}={Value}";
        }
    }
}
