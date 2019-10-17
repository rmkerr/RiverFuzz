using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HttpTokenize.Tokens;
using HttpTokenize;
using System.Net.Http;

namespace UnitTests.HttpTokenize.Tokens
{
    public class PathTokenTests
    {
        [Fact]
        public void PathToken_SimplePath_SegmentReplaced()
        {
            Request request = new Request(new Uri(@"http://test.com/part1/part2"), HttpMethod.Get);
            request.Content = "";

            PathToken token = new PathToken(0, "name", "none", Types.String);

            token.ReplaceValue(request, "replacement");

            Assert.Equal(@"http://test.com/part1/replacement", request.Url.ToString());
        }
    }
}
