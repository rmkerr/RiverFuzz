using CaptureParse.Parsers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTests.CaptureParse
{
    public class ParserFactoryTests
    {
        [Theory]
        [InlineData(ParserConstants.BurpValue)]
        [InlineData(ParserConstants.FiddlerValue)]
        public void ParseFactory_GivenKnownFileType_ReturnsExpectedClass(string inputName)
        {
            var factory = new ParserFactory(this.GetMockParsers());
            var actual = factory.GetParser(inputName).Value;

            Assert.Equal(actual, inputName);
        }

        [Theory]
        [InlineData("Swagger")]
        [InlineData("GIF")]
        [InlineData("Pikachu")]
        [InlineData("???")]
        public void ParseFactory_GivenUnknownFileType_ThrowsException(string inputName)
        {
            var factory = new ParserFactory(this.GetMockParsers());
            var ex = Assert.Throws<ApplicationException>(() => factory.GetParser(inputName));

            //TODO: may want to assert the exception name later
        }

        private IEnumerable<ICaptureParse> GetMockParsers()
        {
            var list = new List<ICaptureParse>();
            var p1 = Substitute.For<ICaptureParse>();
            p1.Value.Returns(ParserConstants.BurpValue);

            var p2 = Substitute.For<ICaptureParse>();
            p2.Value.Returns(ParserConstants.FiddlerValue);

            list.Add(p1);
            list.Add(p2);

            return list;
        }
    }
}
