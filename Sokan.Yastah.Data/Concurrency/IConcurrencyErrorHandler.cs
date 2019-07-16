using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Sokan.Yastah.Data.Concurrency
{
    public interface IConcurrencyErrorHandler<TEntity>
    {
        ConcurrencyResolutionResult HandleConcurrencyError(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues);
    }
}
