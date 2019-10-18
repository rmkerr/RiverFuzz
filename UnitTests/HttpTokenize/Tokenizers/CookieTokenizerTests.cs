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
            request.Headers.Add("Cookie", "test1=test2;test3=test4");

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
    }
}
