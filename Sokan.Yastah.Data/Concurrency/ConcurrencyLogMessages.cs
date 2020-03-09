using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data.Concurrency
{
    public static class ConcurrencyLogMessages
    {
        public enum EventType
        {
            ConcurrencyExceptionHandling        = DataLogEventType.Concurrency + 0x0001,
            ConcurrencyExceptionHandled         = DataLogEventType.Concurrency + 0x0002,
            ConcurrencyExceptionNotHandled      = DataLogEventType.Concurrency + 0x0003,
            ConcurrencyErrorHandlersExecuting   = DataLogEventType.Concurrency + 0x0004,
            ConcurrencyErrorHandlerExecuting    = DataLogEventType.Concurrency + 0x0005,
            EntityEntryHandling                 = DataLogEventType.Concurrency + 0x0006,
            EntityEntryHandled                  = DataLogEventType.Concurrency + 0x0007,
            EntityEntryHandledByHandler         = DataLogEventType.Concurrency + 0x0008,
            EntityEntryNotHandledByHandler      = DataLogEventType.Concurrency + 0x0009
        }

        public static void ConcurrencyErrorHandlerExecuting<TEntity>(
                ILogger logger,
                IConcurrencyErrorHandler<TEntity> handler)
            => _concurrencyErrorHandlerExecuting.Invoke(
                logger,
                handler);
        private static readonly Action<ILogger, object> _concurrencyErrorHandlerExecuting
            = LoggerMessage.Define<object>(
                    LogLevel.Debug,
                    EventType.ConcurrencyErrorHandlerExecuting.ToEventId(),
                    $"Executing {nameof(IConcurrencyErrorHandler<object>)}: {{Handler}}")
                .WithoutException();

        public static void ConcurrencyErrorHandlersExecuting(
                ILogger logger)
            => _concurrencyErrorHandlersExecuting.Invoke(
                logger);
        private static readonly Action<ILogger> _concurrencyErrorHandlersExecuting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.ConcurrencyErrorHandlersExecuting.ToEventId(),
                    $"Executing {nameof(IConcurrencyErrorHandler<object>)} set")
                .WithoutException();

        public static void ConcurrencyExceptionHandled(
                ILogger logger)
            => _concurrencyExceptionHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _concurrencyExceptionHandled
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.ConcurrencyExceptionHandled.ToEventId(),
                    $"{nameof(DbUpdateConcurrencyException)} handled successfully")
                .WithoutException();

        public static void ConcurrencyExceptionHandling(
                ILogger logger,
                DbUpdateConcurrencyException exception)
            => _concurrencyExceptionHandling.Invoke(
                logger,
                exception);
        private static readonly Action<ILogger, Exception> _concurrencyExceptionHandling
            = LoggerMessage.Define(
                LogLevel.Warning,
                EventType.ConcurrencyExceptionHandling.ToEventId(),
                $"Handling {nameof(DbUpdateConcurrencyException)}");

        public static void ConcurrencyExceptionNotHandled(
                ILogger logger,
                DbUpdateConcurrencyException exception)
            => _concurrencyExceptionNotHandled.Invoke(
                logger,
                exception);
        private static readonly Action<ILogger, Exception> _concurrencyExceptionNotHandled
            = LoggerMessage.Define(
                LogLevel.Error,
                EventType.ConcurrencyExceptionNotHandled.ToEventId(),
                $"Unable to handle {nameof(DbUpdateConcurrencyException)}");

        public static void EntityEntryHandled(
                ILogger logger)
            => _entityEntryHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _entityEntryHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.EntityEntryHandled.ToEventId(),
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
                    EventType.EntityEntryHandledByHandler.ToEventId(),
                    $"{nameof(EntityEntry)} handled successfully by handler: {{Handler}}")
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
                    EventType.EntityEntryHandling.ToEventId(),
                    $"Handling {nameof(EntityEntry)}: {{Entry}}")
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
                    EventType.EntityEntryNotHandledByHandler.ToEventId(),
                    $"{nameof(EntityEntry)} not handled by handler: {{Handler}}")
                .WithoutException();
    }
}
