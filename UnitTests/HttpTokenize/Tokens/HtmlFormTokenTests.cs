using HttpTokenize;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace UnitTests.HttpTokenize.Tokens
{
    public class HtmlFormTokenTests
    {
        [Fact]
        public void HtmlFormToken_KeyValue_ReplaceFirstValue()
        {
            string content = "key1=value1&key2=value2";
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get, content);
            request.Headers.Add("Content-Type", new List<string> { "application/x-www-form-urlencoded" });

            HtmlFormToken token1 = new HtmlFormToken("key1", "", Types.String);
            token1.ReplaceValue(request, "testvalue");

            Assert.Equal("key1=testvalue&key2=value2", request.Content);
        }

        [Fact]
        public void HtmlFormToken_KeyValue_ReplaceSecondValue()
        {
            string content = "key1=value1&key2=value2";
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get, content);
            request.Headers.Add("Content-Type", new List<string> { "application/x-www-form-urlencoded" });

            HtmlFormToken token1 = new HtmlFormToken("key2", "", Types.String);
            token1.ReplaceValue(request, "testvalue");

            Assert.Equal("key1=value1&key2=testvalue", request.Content);
        }
    }
}
