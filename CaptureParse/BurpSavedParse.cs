using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using HttpTokenize.RequestSequence;

namespace CaptureParse
{
    public class BurpSavedParse
    {
        public static RequestResponsePair LoadSingleRequestFromFile(string path, string host)
        {
            RequestResponsePair result = null;
            using (StreamReader sr = new StreamReader(path))
            {
                string fileContent = sr.ReadToEnd();
                XElement parsed = XElement.Parse(fileContent);

                XElement requestEncoded = parsed.Descendants("request").First();

                byte[] data = Convert.FromBase64String(requestEncoded.Value);
                string decodedString = Encoding.UTF8.GetString(data);

                Request request = TextParseHelper.ParseRequest(decodedString, host);

                return new RequestResponsePair(request, null);
            }
        }

        public static List<RequestResponsePair> LoadRequestsFromDirectory(string directory, string host)
        {
            List<RequestResponsePair> requests = new List<RequestResponsePair>();

            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string path in filePaths)
            {
                requests.Add(LoadSingleRequestFromFile(path, host));
            }
            return requests;
        }
    }
}
