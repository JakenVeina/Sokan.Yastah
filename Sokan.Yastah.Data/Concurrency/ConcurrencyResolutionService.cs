using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Concurrency
{
    public interface IConcurrencyResolutionService
    {
        Task HandleExceptionAsync(DbUpdateConcurrencyException exception, CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Transient)]
    public class ConcurrencyResolutionService
        : IConcurrencyResolutionService
    {
        public ConcurrencyResolutionService(
            ILogger<ConcurrencyResolutionService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        // For the record, don't even bother trying to Unit Test this. DbUpdateConcurrencyException is not mockable.
        // Layers upon layers of non-mockable dependencies, plus polymorphism abuse, just for extra fun
        // (E.G. if you try and use an IEntityType that isn't EntityType, you get an exception)
        public async Task HandleExceptionAsync(DbUpdateConcurrencyException exception, CancellationToken cancellationToken)
        {
            ConcurrencyLogMessages.ConcurrencyExceptionHandling(_logger, exception);

            foreach (var entry in exception.Entries)
            {
                ConcurrencyLogMessages.EntityEntryHandling(_logger, entry);

                var tEntity = entry.Entity.GetType();
                var originalValues = entry.OriginalValues;
                var currentValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                var proposedValues = entry.CurrentValues;

                if (HandleEntry(tEntity, originalValues, currentValues, proposedValues).IsUnhandled)
                {
                    ConcurrencyLogMessages.ConcurrencyExceptionNotHandled(_logger, exception);
                    throw new InvalidOperationException($"Concurrency exception for entity type {entry.Entity.GetType()} was unhandled", exception);
                }

                ConcurrencyLogMessages.EntityEntryHandled(_logger);
                entry.OriginalValues.SetValues(currentValues);
            }

            ConcurrencyLogMessages.ConcurrencyExceptionHandled(_logger);
        }

        private ConcurrencyResolutionResult HandleEntry(Type tEntity, PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
            => ((Func<PropertyValues, PropertyValues, PropertyValues, ConcurrencyResolutionResult>)
                    GetType()
                        .GetMethod(nameof(HandleEntryGeneric), BindingFlags.Instance | BindingFlags.NonPublic)!
                        .MakeGenericMethod(tEntity)
                        .CreateDelegate(typeof(Func<EntityEntry, PropertyValues, ConcurrencyResolutionResult>), this))
                .Invoke(originalValues, currentValues, proposedValues);

        private ConcurrencyResolutionResult HandleEntryGeneric<TEntity>(PropertyValues originalValues, PropertyValues currentValues, PropertyValues proposedValues)
        {
            var handlers = _serviceProvider
                .GetServices<IConcurrencyErrorHandler<TEntity>>();
            ConcurrencyLogMessages.ConcurrencyErrorHandlersExecuting(_logger);

            var result = ConcurrencyResolutionResult.Unhandled;
            foreach (var handler in handlers)
            {
                ConcurrencyLogMessages.ConcurrencyErrorHandlerExecuting(_logger, handler);

                result = handler.HandleConcurrencyError(originalValues, currentValues, proposedValues);
                if (result.IsHandled)
                {
                    ConcurrencyLogMessages.EntityEntryHandledByHandler(_logger, handler);
                    break;
                }

                ConcurrencyLogMessages.EntityEntryNotHandledByHandler(_logger, handler);
            }

            return result;
        }

        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
    }
}
