using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Microsoft.EntityFrameworkCore
{
    public interface IConcurrencyErrorHandler<TEntity>
    {
        ConcurrencyResolutionResult HandleConcurrencyError(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues);
    }
}
