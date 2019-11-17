using System;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Data.Concurrency;

namespace Sokan.Yastah.Data.Users
{
    public class UserConcurrencyErrorHandler
        : IConcurrencyErrorHandler<UserEntity>
    {
        public ConcurrencyResolutionResult HandleConcurrencyError(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
        {
            if ((DateTimeOffset)currentValues[nameof(UserEntity.LastSeen)] > (DateTimeOffset)proposedValues[nameof(UserEntity.LastSeen)])
                proposedValues.SetValues(currentValues);

            return ConcurrencyResolutionResult.Handled;
        }

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IConcurrencyErrorHandler<UserEntity>, UserConcurrencyErrorHandler>();
    }
}
