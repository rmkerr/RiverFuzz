using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System.Net.Http;

namespace UnitTests.HttpTokenize.Tokenizers
{
    public class CookieTokenizerTests
    {
        [Fact]
        public void CookieTokenizer_SimpleCookieRequest_Parsed()
        {
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get);
            request.Content = "";
            List<String> cookieHeaderVals = new List<string> { "test1=test2;test3=test4" };
            request.Headers.Add("Cookie", cookieHeaderVals);

            CookieTokenizer tokenizer = new CookieTokenizer();

            TokenCollection tokens = tokenizer.ExtractTokens(request);
            Assert.Equal(2, tokens.Count());

            IToken token = tokens.GetByName("test1")[0];
            Assert.Equal("test1", token.Name);
            Assert.Equal("test2", token.Value);

            token = tokens.GetByName("test3")[0];
            Assert.Equal("test3", token.Name);
            Assert.Equal("test4", token.Value);
        }

        [Fact]
        public void CookieTokenizer_SimpleCookieResponse_Parsed()
        {
            Response response = new Response(System.Net.HttpStatusCode.OK, "");
            response.Headers.Add("Set-Cookie", "test_name=test_value; HttpOnly");

            CookieTokenizer tokenizer = new CookieTokenizer();

            TokenCollection tokens = tokenizer.ExtractTokens(response);
            Assert.Equal(1, tokens.Count());

            IToken token = tokens.GetByName("test_name")[0];
            Assert.Equal("test_name", token.Name);
            Assert.Equal("test_value", token.Value);
        }

        [Fact]
        public void CookieTokenizer_MultiCookieResponse_Parsed()
        {
            Response response = new Response(System.Net.HttpStatusCode.OK, "");
            response.Headers.Add("Set-Cookie", "test_name=test_value; HttpOnly");
            response.Headers.Add("Set-Cookie", "test_name2=test_value2; SameSite=Strict");
            response.Headers.Add("Set-Cookie", "test_name3=test_value3; SameSite=Strict; HttpOnly");

            CookieTokenizer tokenizer = new CookieTokenizer();

            TokenCollection tokens = tokenizer.ExtractTokens(response);
            Assert.Equal(3, tokens.Count());

            IToken token = tokens.GetByName("test_name")[0];
            Assert.Equal("test_name", token.Name);
            Assert.Equal("test_value", token.Value);

            token = tokens.GetByName("test_name2")[0];
            Assert.Equal("test_name2", token.Name);
            Assert.Equal("test_value2", token.Value);

            token = tokens.GetByName("test_name3")[0];
            Assert.Equal("test_name3", token.Name);
            Assert.Equal("test_value3", token.Value);
        }
    }
}
