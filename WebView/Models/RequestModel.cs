using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebView.Models
{
    public class RequestModel
    {
        public int RequestModelID { get; set; }
        public EndpointModel Endpoint { get; set; }
        public string Host { get; set; }
        public string RequestText { get; set; }
    }
}
