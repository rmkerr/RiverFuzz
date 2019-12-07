using System;
using Xunit;
using HttpTokenize;
using System.Net.Http;
using HttpTokenize.Tokenizers;
using System.Collections.Generic;
using HttpTokenize.Tokens;

namespace UnitTests
{
    public class RequestTests
    {
        [Fact]
        public void Request_ExtractJsonRequirements_ExpectedValues()
        {
            Request addItemToCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post);
            addItemToCart.Content = "{\"ProductId\":24,\"BasketId\":\"7\",\"quantity\":1}";

            List<IRequestTokenizer> request_tokenizers = new List<IRequestTokenizer>();
            request_tokenizers.Add(new JsonTokenizer());

            TokenCollection requirements = addItemToCart.GetRequirements(request_tokenizers);

            List<IToken> product_id = requirements.GetByName("ProductId");
            Assert.Single(product_id);
            Assert.Equal("ProductId", product_id[0].Name);
            Assert.Equal(Types.Integer, product_id[0].SupportedTypes);

            List<IToken> basket_id = requirements.GetByName("BasketId");
            Assert.Single(basket_id);
            Assert.Equal("BasketId", basket_id[0].Name);
            Assert.Equal(Types.Integer | Types.String, basket_id[0].SupportedTypes);

            List<IToken> quantity = requirements.GetByName("quantity");
            Assert.Single(quantity);
            Assert.Equal("quantity", quantity[0].Name);
            Assert.Equal(Types.Integer, quantity[0].SupportedTypes);
        }
    }
}
