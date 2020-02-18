using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CaptureParse
{
    public class TextCaptureParse
    {
        public static KnownEndpoint LoadSingleRequestFromFile(string path, string host)
        {
            Request request = null;

            using (StreamReader sr = new StreamReader(path))
            {
                request = TextParseHelper.ParseRequest(sr.ReadToEnd(), host);
            }
            return new KnownEndpoint(request, null);
        }

        public static List<KnownEndpoint> LoadRequestsFromDirectory(string directory, string host)
        {
            var requests = new List<KnownEndpoint>();

            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.TopDirectoryOnly);
            foreach (string path in filePaths)
            {
                requests.Add(LoadSingleRequestFromFile(path, host));
            }
            return requests;
        }
    }
}
