using HttpTokenize;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Population.Bucketers
{
    public interface IBucketer
    {
        // Returns true if this results in a new bucket.
        public bool Add(RequestSequence sequence);
        public List<List<RequestSequence>> Bucketize();
    }
}
