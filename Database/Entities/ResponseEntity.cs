using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize;

namespace Database.Entities
{
    public class ResponseEntity
    {
        public int? id { get; set; }
        public string status { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public int? sequence_id { get; set; }
        public int? sequence_position { get; set; }

        public static ResponseEntity FromResponse(Response response)
        {
            ResponseEntity model = new ResponseEntity();
            model.status = response.Status.ToString();

            StringBuilder sb = new StringBuilder();
            foreach (string key in response.Headers.Keys)
            {
                foreach (string value in response.Headers[key])
                {
                    sb.Append(key);
                    sb.Append(": ");
                    sb.Append(value);
                    sb.Append('\n');
                }
            }
            model.headers = sb.ToString();

            model.content = response.Content;

            return model;
        }
    }
}
