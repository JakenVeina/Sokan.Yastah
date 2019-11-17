using System;
using System.Collections.Generic;

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

        public override bool Equals(object? obj)
            => (obj is MergeResult other)
                && Equals(other);

        public bool Equals(MergeResult other)
            => EqualityComparer<int>.Default.Equals(_rowsInserted, other._rowsInserted)
                && EqualityComparer<int>.Default.Equals(_rowsUpdated, other._rowsUpdated);

        public override int GetHashCode()
            => HashCode.Combine(_rowsInserted, _rowsUpdated);

        public static bool operator ==(MergeResult x, MergeResult y)
            => x.Equals(y);

        public static bool operator !=(MergeResult x, MergeResult y)
            => !x.Equals(y);

        private readonly int _rowsInserted;
        private readonly int _rowsUpdated;
    }
}
