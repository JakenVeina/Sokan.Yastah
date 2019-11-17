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

        public static readonly IReadOnlyList<string?> Value_TestCaseData
            = new[]
            {
                null,
                "Value"
            };

        public static readonly IReadOnlyList<TestCaseData> Optional_TestCaseData
            = new[]
            {
                /*                  isSpecified,    value   */
                new TestCaseData(   false,          null    ).SetName("{m}(Unspecified)"),
                new TestCaseData(   true,           null    ).SetName("{m}({1})"),
                new TestCaseData(   true,           "value" ).SetName("{m}({1})")
            };

        public static readonly IReadOnlyList<TestCaseData> OptionalsAreNotEqual_TestCaseData
            = new[]
            {
                /*                  xIsSpecified,   xValue,     yIsSpecified,   yValue      */
                new TestCaseData(   false,          null,       true,           "value 1"   ).SetName("{m}(Unspecified, Specified)"),
                new TestCaseData(   true,           "value 2",  false,          null        ).SetName("{m}(Specified, Unspecified)"),
                new TestCaseData(   true,           "value 3",  true,           "value 4"   ).SetName("{m}(Both Specified, Different Values)")
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

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void FromValue_Always_IsSpecified(
            string? value)
        {
            var result = Optional<string?>.FromValue(value);

            result.IsSpecified.ShouldBeTrue();
            result.IsUnspecified.ShouldBeFalse();
        }

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void FromValue_Always_ValueIsGiven(
            string? value)
        {
            var result = Optional<string?>.FromValue(value);

            result.Value.ShouldBe(value);
        }

        #endregion FromValue() Tests

        #region (Optional<T>)T Tests

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void Operator_CastFromValue_Always_ResultIsFromValue(
            string? value)
        {
            var result = (Optional<string?>)value;

            result.IsSpecified.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion (Optional<T>)T Tests

        #region Equals() Tests

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNull_ReturnsFalse(
            bool isSpecified,
            string value)
        {
            var uut = isSpecified
                ? Optional<string>.FromValue(value!)
                : Optional<string>.Unspecified;

            uut.Equals(null as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotOptional_ReturnsFalse(
            bool isSpecified,
            string value)
        {
            var uut = isSpecified
                ? Optional<string>.FromValue(value!)
                : Optional<string>.Unspecified;

            uut.Equals("obj" as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Equals_NonGeneric_ObjIsEqual_ReturnsTrue(
            bool isSpecified,
            string value)
        {
            var (uut, obj) = isSpecified
                ? (Optional<string>.FromValue(value!),  Optional<string>.FromValue(value!) as object)
                : (Optional<string>.Unspecified,        Optional<string>.Unspecified as object);

            uut.Equals(obj).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OptionalsAreNotEqual_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotEqual_ReturnsFalse(
            bool xIsSpecified,
            string xValue,
            bool yIsSpecified,
            string yValue)
        {
            var uut = xIsSpecified
                ? Optional<string>.FromValue(xValue!)
                : Optional<string>.Unspecified;

            var obj = yIsSpecified
                    ? Optional<string>.FromValue(yValue!)
                    : Optional<string>.Unspecified
                as object;

            uut.Equals(obj).ShouldBeFalse();
        }

        #endregion Equals() Tests

        #region Equals<T>() Tests

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Equals_Generic_OtherIsEqual_ReturnsTrue(
            bool isSpecified,
            string value)
        {
            var (uut, other) = isSpecified
                ? (Optional<string>.FromValue(value!),  Optional<string>.FromValue(value!))
                : (Optional<string>.Unspecified,        Optional<string>.Unspecified);

            uut.Equals(other).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OptionalsAreNotEqual_TestCaseData))]
        public void Equals_Generic_OtherIsNotEqual_ReturnsFalse(
            bool xIsSpecified,
            string xValue,
            bool yIsSpecified,
            string yValue)
        {
            var uut = xIsSpecified
                ? Optional<string>.FromValue(xValue!)
                : Optional<string>.Unspecified;

            var other = yIsSpecified
                ? Optional<string>.FromValue(yValue!)
                : Optional<string>.Unspecified;

            uut.Equals(other).ShouldBeFalse();
        }

        #endregion Equals<T>() Tests

        #region GetHashCode() Tests

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void GetHashCode_UnitsAreEqual_HashCodesAreEqual(
            bool isSpecified,
            string value)
        {
            var (uut, other) = isSpecified
                ? (Optional<string>.FromValue(value!),  Optional<string>.FromValue(value!))
                : (Optional<string>.Unspecified,        Optional<string>.Unspecified);

            uut.GetHashCode().ShouldBe(other.GetHashCode());
        }

        [TestCaseSource(nameof(OptionalsAreNotEqual_TestCaseData))]
        public void GetHashCode_UnitsAreNotEqual_HashCodesAreNotEqual(
            bool xIsSpecified,
            string xValue,
            bool yIsSpecified,
            string yValue)
        {
            var uut = xIsSpecified
                ? Optional<string>.FromValue(xValue!)
                : Optional<string>.Unspecified;

            var other = yIsSpecified
                ? Optional<string>.FromValue(yValue!)
                : Optional<string>.Unspecified;

            uut.GetHashCode().ShouldNotBe(other.GetHashCode());
        }

        #endregion GetHashCode() Tests

        #region == Tests

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Operator_Equals_XAndYAreEqual_ReturnsTrue(
            bool isSpecified,
            string value)
        {
            var (x, y) = isSpecified
                ? (Optional<string>.FromValue(value!),  Optional<string>.FromValue(value!))
                : (Optional<string>.Unspecified,        Optional<string>.Unspecified);

            (x == y).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OptionalsAreNotEqual_TestCaseData))]
        public void Operator_Equals_XAndYAreNotEqual_ReturnsFalse(
            bool xIsSpecified,
            string xValue,
            bool yIsSpecified,
            string yValue)
        {
            var x = xIsSpecified
                ? Optional<string>.FromValue(xValue!)
                : Optional<string>.Unspecified;

            var y = yIsSpecified
                ? Optional<string>.FromValue(yValue!)
                : Optional<string>.Unspecified;

            (x == y).ShouldBeFalse();
        }

        #endregion == Tests

        #region != Tests

        [TestCaseSource(nameof(Optional_TestCaseData))]
        public void Operator_NotEquals_XAndYAreEqual_ReturnsFalse(
            bool isSpecified,
            string value)
        {
            var (x, y) = isSpecified
                ? (Optional<string>.FromValue(value!),  Optional<string>.FromValue(value!))
                : (Optional<string>.Unspecified,        Optional<string>.Unspecified);

            (x != y).ShouldBeFalse();
        }

        [TestCaseSource(nameof(OptionalsAreNotEqual_TestCaseData))]
        public void Operator_NotEquals_XAndYAreNotEqual_ReturnsTrue(
            bool xIsSpecified,
            string xValue,
            bool yIsSpecified,
            string yValue)
        {
            var x = xIsSpecified
                ? Optional<string>.FromValue(xValue!)
                : Optional<string>.Unspecified;

            var y = yIsSpecified
                ? Optional<string>.FromValue(yValue!)
                : Optional<string>.Unspecified;

            (x != y).ShouldBeTrue();
        }

        #endregion != Tests
    }
}
