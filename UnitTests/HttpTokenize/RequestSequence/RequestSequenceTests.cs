using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using HttpTokenize.Substitutions;

namespace UnitTests.HttpTokenize.RequestSequence
{
    public class RequestSequenceTests
    {
        [Fact]
        public void Request_ConstantSubstitution_ExpectedValue()
        {
            Request request = new Request(new Uri(@"http://localhost/rest/user/login/"), HttpMethod.Get);
            request.Content = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";

            IRequestTokenizer tokenizer = new JsonTokenizer();
            TokenCollection tokens = tokenizer.ExtractTokens(request);

            List<IToken> email = tokens.GetByName("email");
            Assert.Single(email);

            ISubstitution substitution = new SubstituteConstant(email[0], "new.email@zmail.com");
            substitution.MakeSubstitution(null, request);

            tokens = tokenizer.ExtractTokens(request);
            email = tokens.GetByName("email");
            Assert.Equal("new.email@zmail.com", email[0].Value);
        }
    }
}
