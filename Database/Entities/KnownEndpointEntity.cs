using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Entities
{
    public class KnownEndpointEntity
    {
        public int? id { get; set; }
        public string url { get; set; }
        public string method { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public string? friendly_name { get; set; }

        public static KnownEndpointEntity FromRequest(Request endpoint)
        {
            KnownEndpointEntity model = new KnownEndpointEntity();
            model.url = endpoint.Url.AbsoluteUri;
            model.method = endpoint.Method.ToString();

            StringBuilder sb = new StringBuilder();
            foreach (string key in endpoint.Headers.Keys)
            {
                foreach (string value in endpoint.Headers[key])
                {
                    sb.Append(key);
                    sb.Append(": ");
                    sb.Append(value);
                    sb.Append('\n');
                }
            }
            model.headers = sb.ToString();

            model.content = endpoint.Content;

            return model;
        }
    }
}