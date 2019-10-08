using HttpTokenize;
using HttpTokenize.Tokenizers;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.HttpTokenize.Tokenizers
{
    public class HtmlFormTokenizerTests
    {
        [Fact]
        public void HtmlForm_UrlEncodedContent_TokensExtracted()
        {
            string content = "key1=value1&key2=value2";
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get, content);
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            HtmlFormTokenizer tokenizer = new HtmlFormTokenizer();

            TokenCollection results = tokenizer.ExtractTokens(request);

            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void HtmlForm_NoToken_NoTokensExtracted()
        {
            string content = "";
            Request request = new Request(new Uri("http://test.com"), HttpMethod.Get, content);
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            HtmlFormTokenizer tokenizer = new HtmlFormTokenizer();

            TokenCollection results = tokenizer.ExtractTokens(request);

            Assert.Equal(0, results.Count());
        }
    }
}
