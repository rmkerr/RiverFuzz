using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using HttpTokenize;
using Population.Bucketers;
using Xunit;

namespace UnitTests.Population.Bucketers
{
    public class ExactStringBucketerTests
    {
        [Fact]
        public void ExactStringBucketer_SimpleStrings_SingleBucket()
        {
            ExactStringBucketer bucketer = new ExactStringBucketer();

            RequestSequence sequence1 = GenerateSequence(1, "matching");
            RequestSequence sequence2 = GenerateSequence(1, "matching");

            bucketer.Add(sequence1);
            bucketer.Add(sequence2);

            Assert.Single(bucketer.Bucketize());
        }

        [Fact]
        public void ExactStringBucketer_SimpleStrings_MultipleBucket()
        {
            ExactStringBucketer bucketer = new ExactStringBucketer();

            RequestSequence sequence1 = GenerateSequence(1, "non-matching 1");
            RequestSequence sequence2 = GenerateSequence(1, "non-matching 2");

            bucketer.Add(sequence1);
            bucketer.Add(sequence2);

            Assert.Equal(2, bucketer.Bucketize().Count);
        }

        RequestSequence GenerateSequence(int numStages, string responseContent)
        {
            RequestSequence sequence = new RequestSequence();
            
            // Force creation of the results list by 'executing' the empty sequence.
            sequence.Execute(null, null, null);

            for (int i = 0; i < numStages; ++i)
            {
                Request request = new Request(new Uri(@"http://localhost/rest/user/login/"), HttpMethod.Get);
                request.Content = "{ \"email\":\"asdf@asdf.com\",\"password\":\"123456\"}";

                sequence.Add(new Stage(request));

                Response response = new Response(System.Net.HttpStatusCode.OK, responseContent);
                sequence.GetResponses().Add(response);
            }

            return sequence;
        }
    }
}
