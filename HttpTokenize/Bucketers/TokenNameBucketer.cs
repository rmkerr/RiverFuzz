using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize.Bucketers
{
    class TokenNameBucketer : IBucketer
    {
        public TokenNameBucketer()
        {
            Responses = new List<Response>();
        }
        public List<Response> Responses { get; }

        public List<List<Response>> Bucketize()
        {
            throw new NotImplementedException();
        }
    }
}
