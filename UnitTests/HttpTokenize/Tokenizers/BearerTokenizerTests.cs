using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Net.Http;

namespace UnitTests
{
    public class BearerTokenizerTests
    {
        private const string BearerExample = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTc0MzEwNSwiZXhwIjoxNTY5NzYxMTA1fQ.Hwjir8myg-rWOpEXlpD-YpA785rY3yRJH24SQkISBYW1MlxnIFmFera3Q48E0VEtlcGSpViBfUCLBFMqMGDdfp5-ujzRrRTq0pHbVjMWqnAMygheO3KYxpvGyY2o1LbAx4EOUksdIGwpxnTRMugVudOWPzZFr89uvKj-Iet6Ig0";

        [Fact]
        public void BearerToken_JsonContent_TokenRecognized()
        {
            string content = "{\"authentication\":{\"token\":\"" + BearerExample + "\",\"bid\":9,\"umail\":\"asdf@asdf.com\"}}";
            Response response = new Response(System.Net.HttpStatusCode.OK, content);

            BearerTokenizer tokenizer = new BearerTokenizer();
            TokenCollection tokens = tokenizer.ExtractTokens(response);

            BearerToken match = new BearerToken(BearerExample);

            Assert.True(tokens.ContainsExactMatch(match) != null);
        }

        [Fact]
        public void BearerToken_RequestHeaders_TokenReplaced()
        {
            Request initializeCart = new Request(new Uri(@"http://localhost/api/BasketItems/"), HttpMethod.Post);
            initializeCart.Content = "{\"ProductId\":24,\"BasketId\":\"20\",\"quantity\":1}";
            initializeCart.Headers.Add("Authorization", new List<string> { $"Bearer {BearerExample}" });

            BearerTokenizer tokenizer = new BearerTokenizer();
            TokenCollection tokens = tokenizer.ExtractTokens(initializeCart);

            List<IToken> match = tokens.GetByName("BearerToken");

            Assert.Single(match);

            match[0].ReplaceValue(initializeCart, "testresult");

            Assert.Equal("Bearer testresult", initializeCart.Headers["Authorization"][0]);
        }
    }
}
