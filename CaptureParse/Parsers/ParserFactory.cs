using System;

namespace CaptureParse.Parsers
{
    public class ParserFactory : IParserFactory
    {
        //TODO: Make an enum and extract or something
        private const string FiddlerFormatName = "Fiddler";
        private const string BurpFormatName = "Burp";

        public ICaptureParse GetParser(string fileFormat)
        {
            ICaptureParse parser;
            if (fileFormat == FiddlerFormatName)
            {
                parser = new TextCaptureParse();
            }
            else if (fileFormat == BurpFormatName)
            {
                parser = new BurpCaptureParse();
            }
            else
            {
                throw new ApplicationException($"Unknown file format {fileFormat}");
            }

            return parser;
        }
    }
}
