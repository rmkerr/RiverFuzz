using System;
using System.Collections.Generic;

namespace CaptureParse.Parsers
{
    public class ParserFactory : IParserFactory
    {
        public Dictionary<string, ICaptureParse> Parsers { get; } = new Dictionary<string, ICaptureParse>();

        public ParserFactory(IEnumerable<ICaptureParse> parsers)
        {
            foreach(var parser in parsers)
            {
                Parsers.Add(parser.Value, parser);
            }
        }

        public ICaptureParse GetParser(string fileFormat)
        {
            ICaptureParse parser;
            if(Parsers.ContainsKey(fileFormat))
            {
                parser = Parsers[fileFormat];
            }
            else
            {
                throw new ApplicationException($"Unknown file format {fileFormat}");
            }

            return parser;
        }
    }
}
