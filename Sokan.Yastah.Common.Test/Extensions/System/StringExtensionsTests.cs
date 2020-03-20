using System;

using NUnit.Framework;
using Moq;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System
{
    [TestFixture]
    public class StringExtensionsTests
    {
        #region ParseInt64() Tests

        [TestCase(long.MinValue)]
        [TestCase(-1L)]
        [TestCase(0L)]
        [TestCase(1L)]
        [TestCase(long.MaxValue)]
        public void ParseInt64_Always_ResultToStringIsValue(long rawValue)
        {
            var value = rawValue.ToString();

            value.ParseInt64().ToString()
                .ShouldBe(value);
        }

        #endregion ParseInt64() Tests

        #region ParseUInt64() Tests

        [TestCase(ulong.MinValue)]
        [TestCase(1UL)]
        [TestCase(ulong.MaxValue)]
        public void ParseUInt64_Always_ResultToStringIsValue(ulong rawValue)
        {
            var value = rawValue.ToString();

            value.ParseUInt64().ToString()
                .ShouldBe(value);
        }

        #endregion ParseUInt64() Tests

        #region ToCamelCaseFromPascalCase() Tests

        [TestCase("",           "")]
        [TestCase("test",       "test")]
        [TestCase("Test",       "test")]
        [TestCase("testValue",  "testValue")]
        [TestCase("TestValue",  "testValue")]
        public void ToCamelCaseFromPascalCase_Always_ReturnsExpected(
                string value,
                string expectedResult)
            => value.ToCamelCaseFromPascalCase()
                .ShouldBe(expectedResult);

        #endregion ToCamelCaseFromPascalCase() Tests
    }
}
