using CaptureParse.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests.CaptureParse
{
    public class ParserFactoryTests
    {
        [Theory]
        [InlineData("Burp", typeof(BurpCaptureParse))]
        [InlineData("Fiddler", typeof(TextCaptureParse))]
        public void ParseFactory_GivenKnownFileType_ReturnsExpectedClass(string inputName, Type expectedType)
        {
            var factory = new ParserFactory();
            var actual = factory.GetParser(inputName).GetType();

            Assert.Equal(actual, expectedType);
        }

        [Theory]
        [InlineData("Swagger")]
        [InlineData("GIF")]
        [InlineData("Pikachu")]
        [InlineData("???")]
        public void ParseFactory_GivenUnknownFileType_ThrowsException(string inputName)
        {
            var factory = new ParserFactory();
            var ex = Assert.Throws<ApplicationException>(() => factory.GetParser(inputName));

            //TODO: may want to assert the exception name later
        }
    }
}
