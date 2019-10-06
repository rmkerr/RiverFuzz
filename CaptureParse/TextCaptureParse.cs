using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CaptureParse
{
    public class TextCaptureParse
    {
        public static Request LoadSingleRequestFromFile(string path, string host)
        {
            Request request = null;
            try
            {
                using (StreamReader sr = new StreamReader(path))
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
                    Dictionary<string, string> headers = new Dictionary<string, string>();
                    while (line != "")
                    {
                        string[] header = line.Split(": ", 2);
                        headers.Add(header[0], header[1]);
                        line = sr.ReadLine();
                    }

                    string content = "";
                    if (!sr.EndOfStream)
                    {
                        content = sr.ReadToEnd();
                    }

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

        public static List<Request> LoadRequestsFromDirectory(string directory, string host)
        {
            List<Request> requests = new List<Request>();

            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string path in filePaths)
            {
                requests.Add(LoadSingleRequestFromFile(path, host));
            }
            return requests;
        }
    }
}
