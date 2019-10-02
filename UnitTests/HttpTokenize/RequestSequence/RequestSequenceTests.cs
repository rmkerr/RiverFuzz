using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace UnitTests.HttpTokenize.RequestSequence
{
    public class RequestSequenceTests
    {
        [Fact]
        public void Request_ConstantSubstitutions_ExpectedValues()
        {
            Request loginUser = new Request();
            loginUser.Url = new Uri(@"http://localhost/rest/user/login/");
            loginUser.Method = HttpMethod.Post;
            loginUser.Content = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";


        }
    }
}
