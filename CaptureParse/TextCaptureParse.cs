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
                    return TextParser.ParseRequest(sr.ReadToEnd(), host);
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
