using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Concurrency
{
    public static class ConcurrencyLogMessages
    {
        public static void ConcurrencyExceptionNotHandled(
                ILogger logger,
                DbUpdateConcurrencyException exception)
            => _concurrencyExceptionNotHandled.Invoke(
                logger,
                exception);
        private static readonly Action<ILogger, Exception> _concurrencyExceptionNotHandled
            = LoggerMessage.Define(
                LogLevel.Error,
                new EventId(1001, nameof(ConcurrencyExceptionNotHandled)),
                $"Unable to handle {nameof(DbUpdateConcurrencyException)}");

        public static void ConcurrencyExceptionHandling(
                ILogger logger,
                DbUpdateConcurrencyException exception)
            => _concurrencyExceptionHandling.Invoke(
                logger,
                exception);
        private static readonly Action<ILogger, Exception> _concurrencyExceptionHandling
            = LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(2001, nameof(ConcurrencyExceptionHandling)),
                $"Handling {nameof(DbUpdateConcurrencyException)}");

        public static void ConcurrencyExceptionHandled(
                ILogger logger)
            => _concurrencyExceptionHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _concurrencyExceptionHandled
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3001, nameof(ConcurrencyExceptionHandled)),
                    $"{nameof(DbUpdateConcurrencyException)} handled successfully")
                .WithoutException();

        public static void EntityEntryHandling(
                ILogger logger,
                EntityEntry entry)
            => _entityEntryHandling.Invoke(
                logger,
                entry);
        private static readonly Action<ILogger, EntityEntry> _entityEntryHandling
            = LoggerMessage.Define<EntityEntry>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(EntityEntryHandling)),
                    $"Handling {nameof(EntityEntry)}: {{Entry}}")
                .WithoutException();

        public static void EntityEntryHandled(
                ILogger logger)
            => _entityEntryHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _entityEntryHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(EntityEntryHandled)),
                    $"{nameof(EntityEntry)} handled successfully")
                .WithoutException();

        public static void EntityEntryHandledByHandler<TEntity>(
                ILogger logger,
                IConcurrencyErrorHandler<TEntity> handler)
            => _entityEntryHandledByHandler.Invoke(
                logger,
                handler);
        private static readonly Action<ILogger, object> _entityEntryHandledByHandler
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(EntityEntryHandledByHandler)),
                    $"{nameof(EntityEntry)} handled successfully by handler: {{Handler}}")
                .WithoutException();

        public static void EntityEntryNotHandledByHandler<TEntity>(
                ILogger logger,
                IConcurrencyErrorHandler<TEntity> handler)
            => _entityEntryNotHandledByHandler.Invoke(
                logger,
                handler);
        private static readonly Action<ILogger, object> _entityEntryNotHandledByHandler
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    new EventId(4004, nameof(EntityEntryNotHandledByHandler)),
                    $"{nameof(EntityEntry)} not handled by handler: {{Handler}}")
                .WithoutException();

        public static void ConcurrencyErrorHandlersExecuting(
                ILogger logger)
            => _concurrencyErrorHandlersExecuting.Invoke(
                logger);
        private static readonly Action<ILogger> _concurrencyErrorHandlersExecuting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4005, nameof(ConcurrencyErrorHandlersExecuting)),
                    $"Executing {nameof(IConcurrencyErrorHandler<object>)} set")
                .WithoutException();

        public static void ConcurrencyErrorHandlerExecuting<TEntity>(
                ILogger logger,
                IConcurrencyErrorHandler<TEntity> handler)
            => _concurrencyErrorHandlerExecuting.Invoke(
                logger,
                handler);
        private static readonly Action<ILogger, object> _concurrencyErrorHandlerExecuting
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    new EventId(4006, nameof(ConcurrencyErrorHandlerExecuting)),
                    $"Executing {nameof(IConcurrencyErrorHandler<object>)}: {{Handler}}")
                .WithoutException();
    }
}
