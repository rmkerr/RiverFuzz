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
        public void PathToken_SimplePath_FirstSegmentReplaced()
        {
            Request request = new Request(new Uri(@"http://test.com/part1/part2"), HttpMethod.Get);
            request.Content = "";

            PathToken token = new PathToken(0, "name", "none", Types.String);

            token.ReplaceValue(request, "replacement");

            Assert.Equal(@"http://test.com/replacement/part2", request.Url.ToString());
        }

        [Fact]
        public void PathToken_SimplePath_LastSegmentReplaced()
        {
            Request request = new Request(new Uri(@"http://test.com/part1/part2"), HttpMethod.Get);
            request.Content = "";

            PathToken token = new PathToken(1, "name", "none", Types.String);

            token.ReplaceValue(request, "replacement");

            Assert.Equal(@"http://test.com/part1/replacement", request.Url.ToString());
        }

        [Fact]
        public void PathToken_SimplePath_LastSegmentRemoved()
        {
            Request request = new Request(new Uri(@"http://test.com/part1/part2"), HttpMethod.Get);
            request.Content = "";

            PathToken token = new PathToken(1, "name", "none", Types.String);

            token.DeleteToken(request);

            Assert.Equal(@"http://test.com/part1", request.Url.ToString());
        }

        [Fact]
        public void PathToken_SimplePath_FirstSegmentRemoved()
        {
            Request request = new Request(new Uri(@"http://test.com/part1/part2"), HttpMethod.Get);
            request.Content = "";

            PathToken token = new PathToken(0, "name", "none", Types.String);

            token.DeleteToken(request);

            Assert.Equal(@"http://test.com/part2", request.Url.ToString());
        }
    }
}
