using System;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    internal static class RepositoryLogMessages
    {
        public enum EventType
        {
            QueryInitializing       = DataLogEventType.Repositories + 0x0001,
            QueryAddingWhereClause  = DataLogEventType.Repositories + 0x0002,
            QueryTerminating        = DataLogEventType.Repositories + 0x0003,
            QueryBuilt              = DataLogEventType.Repositories + 0x0004,
            QueryExecuting          = DataLogEventType.Repositories + 0x0005
        }

        public static void QueryAddingWhereClause(
                ILogger logger,
                string clauseName)
            => _queryAddingWhereClause.Invoke(
                logger,
                clauseName);
        private static readonly Action<ILogger, string> _queryAddingWhereClause
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    EventType.QueryAddingWhereClause.ToEventId(),
                    $"Adding {nameof(Queryable.Where)} clause: {{ClauseName}}")
                .WithoutException();

        public static void QueryBuilt(
                ILogger logger)
            => _queryBuilt.Invoke(
                logger);
        private static readonly Action<ILogger> _queryBuilt
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.QueryBuilt.ToEventId(),
                    "Query built successfully")
                .WithoutException();

        public static void QueryExecuting(
                ILogger logger)
            => _queryExecuting.Invoke(
                logger);
        private static readonly Action<ILogger> _queryExecuting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.QueryExecuting.ToEventId(),
                    "Executing query")
                .WithoutException();

        public static void QueryInitializing<T>(
                ILogger logger,
                IQueryable<T> query)
            => _queryInitializing.Invoke(
                logger,
                query);
        private static readonly Action<ILogger, IQueryable> _queryInitializing
            = LoggerMessage.Define<IQueryable>(
                    LogLevel.Debug,
                    EventType.QueryInitializing.ToEventId(),
                    "Initializing query: {Query}")
                .WithoutException();

        public static void QueryTerminating(
                ILogger logger)
            => _queryTerminating.Invoke(
                logger);
        private static readonly Action<ILogger> _queryTerminating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.QueryTerminating.ToEventId(),
                    "Terminating query")
                .WithoutException();
    }
}
