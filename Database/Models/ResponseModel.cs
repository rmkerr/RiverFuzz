using Database.Models;
using System;
using System.Collections.Generic;
using System.Text;
using HttpTokenize;

namespace DatabaseModels.Models
{
    public class ResponseModel
    {
        public int? id { get; set; }
        public string status { get; set; }
        public string? headers { get; set; }
        public string? content { get; set; }
        public RequestModel? endpoint { get; set; }

        public static ResponseModel FromResponse(Response response)
        {
            ResponseModel model = new ResponseModel();
            model.status = response.Status.ToString();

            StringBuilder sb = new StringBuilder();
            foreach (string key in response.Headers.Keys)
            {
                sb.Append(key);
                sb.Append(": ");
                sb.Append(response.Headers[key]);
                sb.Append('\n');
            }
            model.headers = sb.ToString();

            model.content = response.Content;

            return model;
        }
    }
}
