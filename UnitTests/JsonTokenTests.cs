using System;
using Xunit;
using HttpTokenize.Tokens;
using HttpTokenize;

namespace UnitTests
{
    public class JsonTokenTests
    {
        [Fact]
        public void JsonToken_SingleDepthEmpty_AddsStringToCollection()
        {
            Request request = new Request();
            request.Content = "{}";

            JsonToken token = new JsonToken("test", "test", Types.String);

            token.CopyIntoRequest(request);

            Assert.Equal("{\r\n  \"test\": \"test\"\r\n}", request.Content);
        }

        [Fact]
        public void JsonToken_SingleDepthReplaceName_ReplacesName()
        {
            Request request = new Request();
            request.Content = "{}";

            JsonToken token = new JsonToken("testname", "testval", Types.String);

            token.CopyIntoRequest(request);
            token.ReplaceName(request, "newname");

            Assert.Equal("{\r\n  \"newname\": \"testval\"\r\n}", request.Content);
        }

        [Fact]
        public void JsonToken_SingleDepthReplaceValue_ReplacesName()
        {
            Request request = new Request();
            request.Content = "{}";

            JsonToken token = new JsonToken("testname", "testval", Types.String);

            token.CopyIntoRequest(request);
            token.ReplaceValue(request, "newvalue");

            Assert.Equal("{\r\n  \"testname\": \"newvalue\"\r\n}", request.Content);
        }
    }
}
