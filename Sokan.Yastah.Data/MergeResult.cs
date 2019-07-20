using System;

namespace Sokan.Yastah.Data
{
    public struct MergeResult
    {
        public static readonly MergeResult SingleInsert
            = new MergeResult(1, 0);

        public static readonly MergeResult SingleUpdate
            = new MergeResult(0, 1);

        public MergeResult(int rowsInserted, int rowsUpdated)
        {
            if (rowsInserted < 0)
                throw new ArgumentOutOfRangeException(nameof(rowsInserted), rowsInserted, "Cannot be negative");
            _rowsInserted = rowsInserted;

            if (rowsUpdated < 0)
                throw new ArgumentOutOfRangeException(nameof(rowsUpdated), rowsUpdated, "Cannot be negative");
            _rowsUpdated = rowsUpdated;
        }

        public int RowsInserted
            => _rowsInserted;

        public int RowsUpdated
            => _rowsUpdated;

        private readonly int _rowsInserted;

        private readonly int _rowsUpdated;
    }
}
