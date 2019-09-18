using System.Linq;

using NUnit.Framework;
using Moq;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System.Linq
{
    [TestFixture]
    public class ValueExtensionsTests
    {
        #region ToEnumerable() Tests

        [TestCase(null)]
        [TestCase("")]
        [TestCase("A")]
        [TestCase("This is a test")]
        public void ToEnumerable_Always_YieldsValue(string value)
        {
            var result = value.ToEnumerable()
                .ToArray();

            result.ShouldBeSequenceEqualTo(Enumerable.Empty<string>()
                .Append(value));
        }

        #endregion ToEnumerable() Tests
    }
}
