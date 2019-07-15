using System;

namespace Sokan.Yastah.Data
{
    public struct MergeResult
        : IEquatable<MergeResult>
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

        public bool Equals(MergeResult result)
            => _rowsInserted == result._rowsInserted
                && _rowsUpdated == result._rowsUpdated;

        public override bool Equals(object obj)
            => (obj is MergeResult result)
                ? Equals(result)
                : false;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 + _rowsInserted;
                hash = hash * 16777619 + _rowsUpdated;
                return hash;
            }
        }

        public static bool operator ==(MergeResult x, MergeResult y)
            => x.Equals(y);

        public static bool operator !=(MergeResult x, MergeResult y)
            => !x.Equals(y);

        private readonly int _rowsInserted;

        private readonly int _rowsUpdated;
    }
}
