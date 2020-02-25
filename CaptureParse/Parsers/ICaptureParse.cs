using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaptureParse.Parsers
{
    interface ICaptureParse
    {
        KnownEndpoint ParseSingleRequestFile(string content, string host);
    }
}
