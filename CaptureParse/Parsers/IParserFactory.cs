using System.Collections.Generic;

namespace CaptureParse.Parsers
{
    public interface IParserFactory
    {
        Dictionary<string, ICaptureParse> Parsers { get; }
        ICaptureParse GetParser(string fileFormat);
    }
}