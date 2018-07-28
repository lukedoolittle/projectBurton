using System;
using Burton.Core.Common;
using Xunit;

namespace Burton.Core.Test
{
    public class ExtensionTests
    {
        [Fact]
        public void StringExtensionTest()
        {
            var wholeString = "I sat there with Sally";
            var substring = "I sat";

            var expected = " there with Sally";

            var actual = wholeString.GetStringAfterStartingString(substring);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringExtensionEmptyStringsTest()
        {
            var wholeString = "";
            var substring = "";

            var expected = "";

            var actual = wholeString.GetStringAfterStartingString(substring);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringExtensionEmptyStringTest()
        {
            var wholeString = "I sat";
            var substring = "";

            var expected = "I sat";

            var actual = wholeString.GetStringAfterStartingString(substring);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetNonEmptyTokensTest()
        {
            var target = "I sat there with Sally";

            var actual = target.GetNonEmptyTokens();

            Assert.Equal(5, actual.Count);
        }

        [Fact]
        public void GetNonEmptyTokensWithEmptyStringTest()
        {
            var target = "";

            var actual = target.GetNonEmptyTokens();

            Assert.Empty(actual);
        }
    }
}
