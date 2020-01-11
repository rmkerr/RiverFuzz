using Database;
using Database.Entities;
using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CaptureParse
{
    public class DatabaseParse
    {
        DatabaseHelper DbHelper;
        string Host;
        public DatabaseParse(DatabaseHelper dbHelper, string host)
        {
            DbHelper = dbHelper;
            Host = host;
        }

        public async Task<List<KnownEndpoint>> LoadEndpointsById(List<int> ids)
        {
            List<KnownEndpoint> endpoints = new List<KnownEndpoint>();

            foreach (int i in ids)
            {
                endpoints.Add(await LoadSingleEndpointById(i));
            }

            return endpoints;
        }

        public async Task<KnownEndpoint> LoadSingleEndpointById(int id)
        {
            RequestEntity model = await DbHelper.GetEndpointById(id);

            HttpMethod method = new HttpMethod(model.method);
            Uri url = new Uri(Host + model.url);
            string content = model.content;

            Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
            using (StringReader sr = new StringReader(model.headers))
            {
                string line = sr.ReadLine();
                while (line != "" && line != null)
                {
                    string[] header = line.Split(": ", 2);
                    if (!headers.ContainsKey(header[0]))
                    {
                        headers.Add(header[0], new List<string>());
                    }
                    headers[header[0]].Add(header[1]);
                    line = sr.ReadLine();
                }
            }

            Request request = new Request(url, method, content);
            request.Id = id;

            foreach (string key in headers.Keys)
            {
                request.Headers.Add(key, headers[key]);
            }

            return new KnownEndpoint(request, null);
        }
    }
}
