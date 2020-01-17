using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace CaptureParse
{
    public class BurpSavedParse
    {
        public static KnownEndpoint LoadSingleRequestFromFile(string path, string host)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string fileContent = sr.ReadToEnd();
                return ParseSingleRequestFile(fileContent, host);
            }
        }

        public static List<KnownEndpoint> LoadRequestsFromDirectory(string directory, string host)
        {
            List<KnownEndpoint> requests = new List<KnownEndpoint>();

            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string path in filePaths)
            {
                requests.Add(LoadSingleRequestFromFile(path, host));
            }
            return requests;
        }

        public static KnownEndpoint ParseSingleRequestFile(string content, string host)
        {
            XElement parsed = XElement.Parse(content);

            XElement requestEncoded = parsed.Descendants("request").First();

            byte[] data = Convert.FromBase64String(requestEncoded.Value);
            string decodedString = Encoding.UTF8.GetString(data);

            Request request = TextParseHelper.ParseRequest(decodedString, host);

            return new KnownEndpoint(request, null);
        }
    }
}
