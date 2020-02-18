using System;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Data.Concurrency;

namespace Sokan.Yastah.Data.Users
{
    [ServiceBinding(ServiceLifetime.Scoped)]
    public class UserConcurrencyErrorHandler
        : IConcurrencyErrorHandler<UserEntity>
    {
        public ConcurrencyResolutionResult HandleConcurrencyError(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
        {
            if ((DateTimeOffset)currentValues[nameof(UserEntity.LastSeen)] > (DateTimeOffset)proposedValues[nameof(UserEntity.LastSeen)])
                proposedValues.SetValues(currentValues);

            return ConcurrencyResolutionResult.Handled;
        }
    }
}
