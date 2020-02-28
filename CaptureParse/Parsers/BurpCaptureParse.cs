using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace CaptureParse.Parsers
{
    public class BurpCaptureParse : ICaptureParse
    {
        public string Value => ParserConstants.BurpValue;

        public string DisplayName => ParserConstants.BurpDisplayName;

        public KnownEndpoint LoadSingleRequestFromFile(string path, string host)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string fileContent = sr.ReadToEnd();
                return ParseSingleRequestFile(fileContent, host);
            }
        }

        public List<KnownEndpoint> LoadRequestsFromDirectory(string directory, string host)
        {
            var requests = new List<KnownEndpoint>();

            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string path in filePaths)
            {
                requests.Add(LoadSingleRequestFromFile(path, host));
            }
            return requests;
        }

        public KnownEndpoint ParseSingleRequestFile(string content, string host)
        {
            XElement parsed = XElement.Parse(content);

            XElement requestEncoded = parsed.Descendants("request").First();

            byte[] data = Convert.FromBase64String(requestEncoded.Value);
            string decodedString = Encoding.UTF8.GetString(data);

            Request request = TextParseHelpers.ParseRequest(decodedString, host);

            return new KnownEndpoint(request, null);
        }
    }
}
