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

            /*Assert.Equal("ProductId", requirements[0].Name);
            Assert.Equal(Types.Integer, requirements[0].SupportedTypes);

            Assert.Equal("BasketId", requirements[1].Name);
            Assert.Equal(Types.String, requirements[1].SupportedTypes);

            Assert.Equal("quantity", requirements[2].Name);
            Assert.Equal(Types.Integer, requirements[2].SupportedTypes);*/
        }
    }
}
