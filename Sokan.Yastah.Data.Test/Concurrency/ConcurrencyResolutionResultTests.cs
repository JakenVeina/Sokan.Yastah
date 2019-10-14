using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Data.Concurrency;

namespace Sokan.Yastah.Data.Test.Concurrency
{
    [TestFixture]
    public class ConcurrencyResolutionResultTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<TestCaseData> ResultsAreEqualTestCaseData
            = new[]
            {
                /*                  left                                    right                                   */
                new TestCaseData(   ConcurrencyResolutionResult.Handled,    ConcurrencyResolutionResult.Handled)    .SetName("{m}(Handled == Handled)"),
                new TestCaseData(   ConcurrencyResolutionResult.Unhandled,  ConcurrencyResolutionResult.Unhandled)  .SetName("{m}(Unhandled == Unhandled)"),
            };

        public static readonly IReadOnlyList<TestCaseData> ResultsAreNotEqualTestCaseData
            = new[]
            {
                /*                  left                                    right                                   */
                new TestCaseData(   ConcurrencyResolutionResult.Handled,    ConcurrencyResolutionResult.Unhandled)  .SetName("{m}(Handled == Unhandled)"),
                new TestCaseData(   ConcurrencyResolutionResult.Unhandled,  ConcurrencyResolutionResult.Handled)    .SetName("{m}(Unhandled == Handled)"),
            };

        #endregion Test Cases

        #region Handled Tests

        [Test]
        public void Handled_Always_IsHandled()
        {
            var result = ConcurrencyResolutionResult.Handled;

            result.IsHandled.ShouldBeTrue();
            result.IsUnhandled.ShouldBeFalse();
        }

        #endregion Handled Tests

        #region Unhandled Tests

        [Test]
        public void Unhandled_Always_IsUnhandled()
        {
            var result = ConcurrencyResolutionResult.Unhandled;

            result.IsUnhandled.ShouldBeTrue();
            result.IsHandled.ShouldBeFalse();
        }

        #endregion Unhandled Tests

        #region Equals(ConcurrencyResolutionResult) Tests

        [TestCaseSource(nameof(ResultsAreEqualTestCaseData))]
        public void Equals_ConcurrencyResolutionResult_ResultIsEqual_ReturnsTrue(
            ConcurrencyResolutionResult uut,
            ConcurrencyResolutionResult result)
        {
            uut.Equals(result).ShouldBeTrue();
        }

        [TestCaseSource(nameof(ResultsAreNotEqualTestCaseData))]
        public void Equals_ConcurrencyResolutionResult_ResultIsNotEqual_ReturnsFalse(
            ConcurrencyResolutionResult uut,
            ConcurrencyResolutionResult result)
        {
            uut.Equals(result).ShouldBeFalse();
        }

        #endregion Equals(ConcurrencyResolutionResult) Tests

        #region Equals(object) Tests

        [TestCase(null)]
        [TestCase(1)]
        [TestCase("test")]
        public void Equals_Object_ObjIsNotResult_ReturnsFalse(
            object obj)
        {
            var uut = ConcurrencyResolutionResult.Handled;

            uut.Equals(obj).ShouldBeFalse();
        }

        [TestCaseSource(nameof(ResultsAreEqualTestCaseData))]
        public void Equals_Object_ObjIsResultAndEqual_ReturnsTrue(
            ConcurrencyResolutionResult uut,
            ConcurrencyResolutionResult result)
        {
            var obj = result as object;

            uut.Equals(obj).ShouldBeTrue();
        }

        [TestCaseSource(nameof(ResultsAreNotEqualTestCaseData))]
        public void Equals_Object_ObjIsResultAndNotEqual_ReturnsFalse(
            ConcurrencyResolutionResult uut,
            ConcurrencyResolutionResult result)
        {
            var obj = result as object;

            uut.Equals(obj).ShouldBeFalse();
        }

        #endregion Equals(object) Tests

        #region GetHashCode() Tests

        [TestCaseSource(nameof(ResultsAreEqualTestCaseData))]
        public void GetHashCode_ResultsAreEqual_HashCodesAreEqual(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            left.GetHashCode().ShouldBe(right.GetHashCode());
        }

        [TestCaseSource(nameof(ResultsAreNotEqualTestCaseData))]
        public void GetHashCode_ResultsAreNotEqual_HashCodesAreNotEqual(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            left.GetHashCode().ShouldNotBe(right.GetHashCode());
        }

        #endregion GetHashCode() Tests

        #region == Tests

        [TestCaseSource(nameof(ResultsAreEqualTestCaseData))]
        public void OperatorEquals_ValuesAreEqual_ReturnsTrue(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            (left == right).ShouldBeTrue();
        }

        [TestCaseSource(nameof(ResultsAreNotEqualTestCaseData))]
        public void OperatorEquals_ValuesAreNotEqual_ReturnsFalse(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            (left == right).ShouldBeFalse();
        }

        #endregion == Tests

        #region != Tests

        [TestCaseSource(nameof(ResultsAreEqualTestCaseData))]
        public void OperatorNotEquals_ValuesAreEqual_ReturnsFalse(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            (left != right).ShouldBeFalse();
        }

        [TestCaseSource(nameof(ResultsAreNotEqualTestCaseData))]
        public void OperatorNotEquals_ValuesAreNotEqual_ReturnsTrue(
            ConcurrencyResolutionResult left,
            ConcurrencyResolutionResult right)
        {
            (left != right).ShouldBeTrue();
        }

        #endregion != Tests
    }
}
