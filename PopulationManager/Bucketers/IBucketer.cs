using HttpTokenize;
using HttpTokenize.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace PopulationManager.Bucketers
{
    public interface IBucketer
    {
        // Returns true if this results in a new bucket.
        public bool Add(Response response, TokenCollection tokens);
        public List<List<Response>> Bucketize();
    }
}
