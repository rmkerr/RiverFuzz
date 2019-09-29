using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;

namespace HttpTokenize
{
    public class Response
    {
        public Response(HttpStatusCode status, string content)
        {
            Status = status;
            Content = content;
        }

        public HttpStatusCode Status { get; }
        public string Content { get; }
    }
}
