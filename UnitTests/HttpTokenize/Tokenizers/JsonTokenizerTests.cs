using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HttpTokenize.Tokenizers;
using HttpTokenize;
using HttpTokenize.Tokens;
using System.Net.Http;

namespace UnitTests.HttpTokenize.Tokenizers
{
    public class JsonTokenizerTests
    {
        [Fact]
        public void JsonTokenizer_SimpleJson_Tokenized()
        {
            Request addItemToCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post);
            addItemToCart.Content = "{\"ProductId\":24,\"BasketId\":\"7\",\"quantity\":1}";

            List<IRequestTokenizer> request_tokenizers = new List<IRequestTokenizer>();
            request_tokenizers.Add(new JsonTokenizer());

            TokenCollection tokens = addItemToCart.GetRequirements(request_tokenizers);
            Assert.Equal(3, tokens.Count());

            IToken token = tokens.GetByName("ProductId")[0];
            Assert.Equal("24", token.Value);
            Assert.Equal(Types.Integer, token.SupportedTypes);

            token = tokens.GetByName("BasketId")[0];
            Assert.Equal("7", token.Value);
            Assert.Equal(Types.Integer | Types.String, token.SupportedTypes);

            token = tokens.GetByName("quantity")[0];
            Assert.Equal("1", token.Value);
            Assert.Equal(Types.Integer, token.SupportedTypes);
        }
    }
}
