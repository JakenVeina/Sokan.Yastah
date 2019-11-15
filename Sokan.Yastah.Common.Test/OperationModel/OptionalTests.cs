using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class OptionalTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string?> ValueTestCases
            = new[]
            {
                null,
                "Value"
            };

        #endregion Test Cases

        #region Default Tests

        [Test]
        public void Default_Always_IsUnspecified()
        {
            var result = default(Optional<string>);

            result.IsSpecified.ShouldBeFalse();
            result.IsUnspecified.ShouldBeTrue();
        }

        [Test]
        public void Default_Always_ValueThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = default(Optional<string>).Value;
            });
        }

        #endregion Default Tests

        #region Unspecified Tests

        [Test]
        public void Unspecified_Always_IsUnspecified()
        {
            var result = Optional<string>.Unspecified;

            result.IsSpecified.ShouldBeFalse();
            result.IsUnspecified.ShouldBeTrue();
        }

        [Test]
        public void Unspecified_Always_ValueThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = Optional<string>.Unspecified.Value;
            });
        }

        #endregion Unspecified Tests

        #region FromValue() Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_IsSpecified(
            string? value)
        {
            var result = Optional<string?>.FromValue(value);

            result.IsSpecified.ShouldBeTrue();
            result.IsUnspecified.ShouldBeFalse();
        }

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_ValueIsGiven(
            string? value)
        {
            var result = Optional<string?>.FromValue(value);

            result.Value.ShouldBe(value);
        }

        #endregion FromValue() Tests

        #region (Optional<T>)T Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void Operator_CastFromValue_Always_ResultIsFromValue(
            string? value)
        {
            var result = (Optional<string?>)value;

            result.IsSpecified.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion (Optional<T>)T Tests
    }
}
