using System;

namespace CaptureParse.Parsers
{
    public class ParserFactory : IParserFactory
    {
        public ICaptureParse GetParser(string fileFormat)
        {
            ICaptureParse parser;
            if (fileFormat == ParserConstants.FiddlerValue)
            {
                parser = new TextCaptureParse();
            }
            else if (fileFormat == ParserConstants.BurpValue)
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
