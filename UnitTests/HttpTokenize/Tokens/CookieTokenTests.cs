using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize.Tokens;
using HttpTokenize;
using System.Net.Http;
using Xunit;

namespace UnitTests.HttpTokenize.Tokens
{
    public class CookieTokenTests
    {
        [Fact]
        public void CookieToken_SimpleCookie_FirstValueReplaced()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "";
            request.Headers.Add("Cookie", "test1=test2;test3=test4");

            CookieToken token = new CookieToken("test1", "", Types.String);
            token.ReplaceValue(request, "newValue");

            Assert.Equal("test1=newValue;test3=test4", request.Headers["Cookie"]);
        }

        [Fact]
        public void CookieToken_SimpleCookie_LastValueReplaced()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "";
            request.Headers.Add("Cookie", "test1=test2;test3=test4");

            CookieToken token = new CookieToken("test3", "", Types.String);
            token.ReplaceValue(request, "newValue");

            Assert.Equal("test1=test2;test3=newValue", request.Headers["Cookie"]);
        }
    }
}
