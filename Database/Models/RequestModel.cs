using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Models
{
    public class RequestModel
    {
        public int? id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }

        public static RequestModel FromRequest(Request endpoint)
        {
            RequestModel model = new RequestModel();
            model.url = endpoint.Url.AbsoluteUri;
            model.method = endpoint.Method.ToString();

            StringBuilder sb = new StringBuilder();
            foreach (string key in endpoint.Headers.Keys)
            {
                sb.Append(key);
                sb.Append(": ");
                sb.Append(endpoint.Headers[key]);
                sb.Append('\n');
            }
            model.headers = sb.ToString();

            model.content = endpoint.Content;

            return model;
        }
    }
}
