using Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace UnitTests.Config
{
    public class FuzzConfigParseTests
    {
        [Fact]
        public void FuzzRunOptions_GivenValues_ParsesSuccessfully()
        {
            string json = File.ReadAllText("./Json/fuzzRequestOptions.json");
            var actual = new FuzzRunOptions(json);

            var endpointSequence = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            Assert.Equal(TimeSpan.FromMinutes(2), actual.ExecutionTime);
            Assert.Equal("http://localhost", actual.Target);
            Assert.Equal(endpointSequence, actual.TargetEndpointIds);
            Assert.Equal("Test", actual.RunName);
        }
    }
}
