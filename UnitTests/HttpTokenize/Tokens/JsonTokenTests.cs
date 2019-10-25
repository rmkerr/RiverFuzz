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
        public void JsonToken_SingleDepthReplaceName_ReplacesName()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "{\r\n  \"testname\": \"testval\"\r\n}";

            JsonToken token = new JsonToken("testname", "testval", "testname", Types.String);

            token.ReplaceName(request, "newname");

            Assert.Equal("{\r\n  \"newname\": \"testval\"\r\n}", request.Content);
        }

        [Fact]
        public void JsonToken_SingleDepthReplaceValue_ReplacesValue()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "{\r\n  \"testname\": \"testval\"\r\n}";

            JsonToken token = new JsonToken("testname", "testval", "testname", Types.String);

            token.ReplaceValue(request, "newvalue");

            Assert.Equal("{\r\n  \"testname\": \"newvalue\"\r\n}", request.Content);
        }

        [Fact]
        public void JsonToken_SingleDepthRemove_RemovesToken()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "{\r\n  \"testname\": \"testval\"\r\n}";

            JsonToken token = new JsonToken("testname", "testval", "testname", Types.String);

            token.Remove(request);

            Assert.Equal("{}", request.Content);
        }
    }
}
