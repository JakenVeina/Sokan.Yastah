using System;

using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System
{

    [TestFixture]
    public class StructExtensionsTests
    {
        #region ToNullable() Tests

        [TestCase(default(int))]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ToNullable_Always_CastsValueToNullable(
                int value)
            => value.ToNullable()
                .ShouldBe(value);

        #endregion ToNullable() Tests
    }
}
