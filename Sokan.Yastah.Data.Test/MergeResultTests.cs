using System;

using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Data.Test
{
    [TestFixture]
    public class MergeResultTests
    {
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

        [TestCase(0,            0)]
        [TestCase(1,            4)]
        [TestCase(2,            5)]
        [TestCase(3,            6)]
        [TestCase(int.MaxValue, int.MaxValue)]
        public void Constructor_Otherwise_RowsInsertedAndRowsUpdatedAreGiven(
            int rowsInserted,
            int rowsUpdated)
        {
            var result = new MergeResult(rowsInserted, rowsUpdated);

            result.RowsInserted.ShouldBe(rowsInserted);
            result.RowsUpdated.ShouldBe(rowsUpdated);
        }

        #endregion Constructor() Tests
    }
}
