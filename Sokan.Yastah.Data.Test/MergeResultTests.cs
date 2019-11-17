using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class MergeResultTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<TestCaseData> MergeResult_TestCaseData
            = new[]
            {
                /*                  rowsInserted,   rowsUpdated     */
                new TestCaseData(   default(int),   default(int)    ).SetName("{m}(Default Values)"),
                new TestCaseData(   1,              2               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3,              4               ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5,              6               ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   int.MaxValue    ).SetName("{m}(Max Values)")
            };

        public static readonly IReadOnlyList<TestCaseData> MergeResultsAreNotEqual_TestCaseData
            = new[]
            {
                /*                  xRowsInserted,  xRowsUpdated    yRowsInserted,  yRowsUpdated    */
                new TestCaseData(   1,              2,              1,              3               ).SetName("{m}(RowsInserted values are Equal)"),
                new TestCaseData(   4,              5,              6,              5               ).SetName("{m}(RowsUpdated values are Equal)"),
                new TestCaseData(   7,              8,              9,              10              ).SetName("{m}(No values are Equal)")
            };

        #endregion Test Cases

        #region SingleInsert Tests

        [Test]
        public void SingleInsert_Always_IsSingleInsert()
        {
            MergeResult.SingleInsert.RowsInserted.ShouldBe(1);
            MergeResult.SingleInsert.RowsUpdated.ShouldBe(0);
        }

        #endregion SingleInsert Tests

        #region SingleUpdate Tests

        [Test]
        public void SingleUpdate_Always_IsSingleUpdate()
        {
            MergeResult.SingleUpdate.RowsInserted.ShouldBe(0);
            MergeResult.SingleUpdate.RowsUpdated.ShouldBe(1);
        }

        #endregion SingleUpdate Tests

        #region Constructor() Tests

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        public void Constructor_RowsInsertedIsNegative_ThrowsException(
            int rowsInserted)
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new MergeResult(rowsInserted, 0);
            });
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        public void Constructor_RowsUpdatedIsNegative_ThrowsException(
            int rowsUpdated)
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
            {
                new MergeResult(0, rowsUpdated);
            });
        }

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Constructor_Otherwise_RowsInsertedAndRowsUpdatedAreGiven(
            int rowsInserted,
            int rowsUpdated)
        {
            var result = new MergeResult(rowsInserted, rowsUpdated);

            result.RowsInserted.ShouldBe(rowsInserted);
            result.RowsUpdated.ShouldBe(rowsUpdated);
        }

        #endregion Constructor() Tests

        #region Equals() Tests

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNull_ReturnsFalse(
            int rowsInserted,
            int rowsUpdated)
        {
            var uut = new MergeResult(rowsInserted, rowsUpdated);

            uut.Equals(null as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotMergeResult_ReturnsFalse(
            int rowsInserted,
            int rowsUpdated)
        {
            var uut = new MergeResult(rowsInserted, rowsUpdated);

            uut.Equals("obj" as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsEqual_ReturnsTrue(
            int rowsInserted,
            int rowsUpdated)
        {
            var uut = new MergeResult(rowsInserted, rowsUpdated);
            var obj = new MergeResult(rowsInserted, rowsUpdated);

            uut.Equals(obj).ShouldBeTrue();
        }

        [TestCaseSource(nameof(MergeResultsAreNotEqual_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotEqual_ReturnsFalse(
            int xRowsInserted,
            int xRowsUpdated,
            int yRowsInserted,
            int yRowsUpdated)
        {
            var uut = new MergeResult(xRowsInserted, xRowsUpdated);
            var obj = new MergeResult(yRowsInserted, yRowsUpdated)
                as object;

            uut.Equals(obj).ShouldBeFalse();
        }

        #endregion Equals() Tests

        #region Equals<T>() Tests

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Equals_Generic_OtherIsEqual_ReturnsTrue(
            int rowsInserted,
            int rowsUpdated)
        {
            var uut = new MergeResult(rowsInserted, rowsUpdated);
            var other = new MergeResult(rowsInserted, rowsUpdated);

            uut.Equals(other).ShouldBeTrue();
        }

        [TestCaseSource(nameof(MergeResultsAreNotEqual_TestCaseData))]
        public void Equals_Generic_OtherIsNotEqual_ReturnsFalse(
            int xRowsInserted,
            int xRowsUpdated,
            int yRowsInserted,
            int yRowsUpdated)
        {
            var uut = new MergeResult(xRowsInserted, xRowsUpdated);
            var other = new MergeResult(yRowsInserted, yRowsUpdated);

            uut.Equals(other).ShouldBeFalse();
        }

        #endregion Equals<T>() Tests

        #region GetHashCode() Tests

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void GetHashCode_UnitsAreEqual_HashCodesAreEqual(
            int rowsInserted,
            int rowsUpdated)
        {
            var uut = new MergeResult(rowsInserted, rowsUpdated);
            var other = new MergeResult(rowsInserted, rowsUpdated);

            uut.GetHashCode().ShouldBe(other.GetHashCode());
        }

        [TestCaseSource(nameof(MergeResultsAreNotEqual_TestCaseData))]
        public void GetHashCode_UnitsAreNotEqual_HashCodesAreNotEqual(
            int xRowsInserted,
            int xRowsUpdated,
            int yRowsInserted,
            int yRowsUpdated)
        {
            var uut = new MergeResult(xRowsInserted, xRowsUpdated);
            var other = new MergeResult(yRowsInserted, yRowsUpdated);

            uut.GetHashCode().ShouldNotBe(other.GetHashCode());
        }

        #endregion GetHashCode() Tests

        #region == Tests

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Operator_Equals_XAndYAreEqual_ReturnsTrue(
            int rowsInserted,
            int rowsUpdated)
        {
            var x = new MergeResult(rowsInserted, rowsUpdated);
            var y = new MergeResult(rowsInserted, rowsUpdated);

            (x == y).ShouldBeTrue();
        }

        [TestCaseSource(nameof(MergeResultsAreNotEqual_TestCaseData))]
        public void Operator_Equals_XAndYAreNotEqual_ReturnsFalse(
            int xRowsInserted,
            int xRowsUpdated,
            int yRowsInserted,
            int yRowsUpdated)
        {
            var x = new MergeResult(xRowsInserted, xRowsUpdated);
            var y = new MergeResult(yRowsInserted, yRowsUpdated);

            (x == y).ShouldBeFalse();
        }

        #endregion == Tests

        #region != Tests

        [TestCaseSource(nameof(MergeResult_TestCaseData))]
        public void Operator_NotEquals_XAndYAreEqual_ReturnsFalse(
            int rowsInserted,
            int rowsUpdated)
        {
            var x = new MergeResult(rowsInserted, rowsUpdated);
            var y = new MergeResult(rowsInserted, rowsUpdated);

            (x != y).ShouldBeFalse();
        }

        [TestCaseSource(nameof(MergeResultsAreNotEqual_TestCaseData))]
        public void Operator_NotEquals_XAndYAreNotEqual_ReturnsTrue(
            int xRowsInserted,
            int xRowsUpdated,
            int yRowsInserted,
            int yRowsUpdated)
        {
            var x = new MergeResult(xRowsInserted, xRowsUpdated);
            var y = new MergeResult(yRowsInserted, yRowsUpdated);

            (x != y).ShouldBeTrue();
        }

        #endregion != Tests
    }
}
