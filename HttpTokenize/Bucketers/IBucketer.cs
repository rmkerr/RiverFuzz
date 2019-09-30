using System;
using System.Collections.Generic;
using System.Text;

namespace HttpTokenize
{
    public interface IBucketer
    {
        public List<Response> Responses { get; }
        public List<List<Response>> Bucketize();
    }
}
