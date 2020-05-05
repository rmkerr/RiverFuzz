using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HttpTokenize;
using HttpTokenize.Tokens;

namespace UnitTests.HttpTokenize
{
    public class TypeGuesserTests
    {
        [Theory]
        [InlineData("1234", Types.Integer)]
        [InlineData("-1234", Types.Integer)]
        [InlineData("0", Types.Integer | Types.Boolean)]
        [InlineData("1", Types.Integer | Types.Boolean)]
        [InlineData("True", Types.Boolean)]
        [InlineData("false", Types.Boolean)]
        [InlineData("Test string", Types.String)]
        [InlineData("http://test.com/", Types.Url | Types.String)]
        [InlineData("https://test.com/", Types.Url | Types.String)]
        [InlineData("ws://test.com/", Types.Url | Types.String)]
        [InlineData("wss://test.com/", Types.Url | Types.String)]
        void TypeGuesser_SimpleData_TypeInferredCorrectly(string val, Types expected)
        {
            Types types = TypeGuesser.GuessTypes(val);
            Assert.Equal(expected, types);
        }
    }
}
