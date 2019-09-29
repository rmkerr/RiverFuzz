using HttpTokenize;
using HttpTokenize.Tokens;
using HttpTokenize.Tokenizers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class BearerTokenizerTests
    {
        [Fact]
        public void JsonToken_SingleDepthEmpty_AddsStringToCollection()
        {
            string content = "{\"authentication\":{\"token\":\"eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdGF0dXMiOiJzdWNjZXNzIiwiZGF0YSI6eyJpZCI6MTYsInVzZXJuYW1lIjoiIiwiZW1haWwiOiJhc2RmQGFzZGYuY29tIiwicGFzc3dvcmQiOiJlMTBhZGMzOTQ5YmE1OWFiYmU1NmUwNTdmMjBmODgzZSIsInJvbGUiOiJjdXN0b21lciIsImxhc3RMb2dpbklwIjoiMTI3LjAuMC4xIiwicHJvZmlsZUltYWdlIjoiZGVmYXVsdC5zdmciLCJ0b3RwU2VjcmV0IjoiIiwiaXNBY3RpdmUiOnRydWUsImNyZWF0ZWRBdCI6IjIwMTktMDktMjggMDQ6NTA6NDYuMjc3ICswMDowMCIsInVwZGF0ZWRBdCI6IjIwMTktMDktMjggMTk6MTI6MDMuNjIzICswMDowMCIsImRlbGV0ZWRBdCI6bnVsbH0sImlhdCI6MTU2OTc0MzEwNSwiZXhwIjoxNTY5NzYxMTA1fQ.Hwjir8myg-rWOpEXlpD-YpA785rY3yRJH24SQkISBYW1MlxnIFmFera3Q48E0VEtlcGSpViBfUCLBFMqMGDdfp5-ujzRrRTq0pHbVjMWqnAMygheO3KYxpvGyY2o1LbAx4EOUksdIGwpxnTRMugVudOWPzZFr89uvKj-Iet6Ig0\",\"bid\":9,\"umail\":\"asdf@asdf.com\"}}";
            Response response = new Response(System.Net.HttpStatusCode.OK, content);

            

            //Assert.Equal("{\r\n  \"test\": \"test\"\r\n}", request.Content);
        }
    }
}
