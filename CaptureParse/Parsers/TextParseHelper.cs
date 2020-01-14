using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace CaptureParse
{
    internal class TextParseHelper
    {
        public static Request ParseRequest(string requestString, string host)
        {
            Request request = null;
            try
            {
                using (StringReader sr = new StringReader(requestString))
                {
                    string line = sr.ReadLine();
                    Console.WriteLine(line);
                    string[] contents = line.Split(' ');

                    if (contents.Length != 3)
                    {
                        throw new InvalidOperationException("Invalid HTTP request line.");
                    }

                    HttpMethod method = new HttpMethod(contents[0]);

                    Uri url = new Uri(host + contents[1]);

                    line = sr.ReadLine();
                    Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();
                    while (line != "")
                    {
                        string[] header = line.Split(": ", 2);
                        if (!headers.ContainsKey(header[0]))
                        {
                            headers.Add(header[0], new List<string>());
                        }
                        headers[header[0]].Add(header[1]);
                        line = sr.ReadLine();
                    }

                    string content = sr.ReadToEnd();

                    // TODO: Null content vs emty string content.
                    request = new Request(url, method, content);

                    foreach (string key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return request;
        }
    }
}
