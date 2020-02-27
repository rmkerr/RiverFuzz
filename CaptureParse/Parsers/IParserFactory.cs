namespace CaptureParse.Parsers
{
    public interface IParserFactory
    {
        ICaptureParse GetParser(string fileFormat);
    }
}