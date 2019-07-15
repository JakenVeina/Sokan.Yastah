using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    public interface IConcurrencyResolutionService
    {
        Task SaveConcurrentChangesAsync(DbContext dbContext, CancellationToken cancellationToken);
    }

    public class ConcurrencyResolutionService
        : IConcurrencyResolutionService
    {
        public ConcurrencyResolutionService(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task SaveConcurrentChangesAsync(DbContext dbContext, CancellationToken cancellationToken)
        {
            // This seems dangerous, but simply serves to handle additional updates being performed to the entity while we are calculating the resolution.
            // Because we manually overwrite OriginalValues with DatabaseValues below, after the resolution, the only way the next call to SaveChangesAsync()
            // can fails is if an additional concurrent update occurs.
            var success = false;
            while(!success)
            {
                try
                {
                    await dbContext
                        .SaveChangesAsync(cancellationToken);
                    success = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        var tEntity = entry.Entity.GetType();
                        var originalValues = entry.OriginalValues;
                        var currentValues = await entry.GetDatabaseValuesAsync();
                        var proposedValues = entry.CurrentValues;

                        if (HandleEntry(tEntity, originalValues, currentValues, proposedValues).IsUnhandled)
                            throw new InvalidOperationException($"Concurrency exception for entity type {entry.Entity.GetType()} was unhandled", ex);

                        entry.OriginalValues.SetValues(currentValues);
                    }
                }
            }
        }

        private ConcurrencyResolutionResult HandleEntry(Type tEntity, PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
            => (GetType()
                        .GetMethod(nameof(HandleEntryGeneric), BindingFlags.Instance | BindingFlags.NonPublic)
                        .MakeGenericMethod(tEntity)
                        .CreateDelegate(typeof(Func<EntityEntry, PropertyValues, ConcurrencyResolutionResult>), this)
                    as Func<PropertyValues, PropertyValues, PropertyValues, ConcurrencyResolutionResult>)
                .Invoke(originalValues, currentValues, proposedValues);

        private ConcurrencyResolutionResult HandleEntryGeneric<TEntity>(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
        {
            var handlers = _serviceProvider
                .GetServices<IConcurrencyErrorHandler<TEntity>>();

            var result = ConcurrencyResolutionResult.Unhandled;
            foreach (var handler in handlers)
            {
                result = handler.HandleConcurrencyError(originalValues, currentValues, proposedValues);
                if (result.IsHandled)
                    break;
            }

            return result;
        }

        private readonly IServiceProvider _serviceProvider;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IConcurrencyResolutionService, ConcurrencyResolutionService>();
    }
}
