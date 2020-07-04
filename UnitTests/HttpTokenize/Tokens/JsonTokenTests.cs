using System;
using Xunit;
using HttpTokenize.Tokens;
using HttpTokenize;
using System.Net.Http;

namespace UnitTests
{
    public class JsonTokenTests
    {
        [Fact]
        public void JsonToken_SingleDepthReplaceValue_ReplacesValue()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "{\r\n  \"testname\": \"testval\"\r\n}";

            JsonToken token = new JsonToken("testname", "testval", "testname", Types.String);

            token.ReplaceValue(request, "newvalue");

            // On linux, json.net uses LF instead of CRLF. This is probably the correct behavior,
            // but we need to normalize for the tests.
            string content = request.Content.Replace("\r\n", "\n");

            Assert.Equal("{\n  \"testname\": \"newvalue\"\n}", content);
        }

        [Fact]
        public void JsonToken_SingleDepthRemove_RemovesToken()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "{\r\n  \"testname\": \"testval\"\r\n}";

            JsonToken token = new JsonToken("testname", "testval", "testname", Types.String);

            token.DeleteToken(request);

            Assert.Equal("{}", request.Content);
        }
    }
}
