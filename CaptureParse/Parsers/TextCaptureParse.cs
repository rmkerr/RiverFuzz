using HttpTokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CaptureParse.Parsers
{
    public class TextCaptureParse : ICaptureParse
    {
        public string Value => ParserConstants.FiddlerValue;

        public string DisplayName => ParserConstants.FiddlerDiplayName;

        public KnownEndpoint LoadSingleRequestFromFile(string path, string host)
        {
            Request request = null;

            using (StreamReader sr = new StreamReader(path))
            {
                request = TextParseHelpers.ParseRequest(sr.ReadToEnd(), host);
            }
            return new KnownEndpoint(request, null);
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
            Request request = TextParseHelpers.ParseRequest(content, host);
            return new KnownEndpoint(request, null);
        }
    }
}
