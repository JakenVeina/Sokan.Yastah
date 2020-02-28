using System;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Data
{
    internal static class RepositoryLogMessages
    {
        public static void QueryInitializing<T>(
                ILogger logger,
                IQueryable<T> query)
            => _queryInitializing.Invoke(
                logger,
                query);
        private static readonly Action<ILogger, IQueryable> _queryInitializing
            = LoggerMessage.Define<IQueryable>(
                    LogLevel.Debug,
                    new EventId(4101, nameof(QueryInitializing)),
                    "Initializing query: {Query}")
                .WithoutException();

        public static void QueryAddingWhereClause(
                ILogger logger,
                string clauseName)
            => _queryAddingWhereClause.Invoke(
                logger,
                clauseName);
        private static readonly Action<ILogger, string> _queryAddingWhereClause
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(4102, nameof(QueryAddingWhereClause)),
                    $"Adding {nameof(Queryable.Where)} clause: {{ClauseName}}")
                .WithoutException();

        public static void QueryTerminating(
                ILogger logger)
            => _queryTerminating.Invoke(
                logger);
        private static readonly Action<ILogger> _queryTerminating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4103, nameof(QueryTerminating)),
                    "Terminating query")
                .WithoutException();

        public static void QueryBuilt(
                ILogger logger)
            => _queryBuilt.Invoke(
                logger);
        private static readonly Action<ILogger> _queryBuilt
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4104, nameof(QueryBuilt)),
                    "Query built successfully")
                .WithoutException();

        public static void QueryExecuting(
                ILogger logger)
            => _queryExecuting.Invoke(
                logger);
        private static readonly Action<ILogger> _queryExecuting
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4105, nameof(QueryExecuting)),
                    "Executing query")
                .WithoutException();
    }
}
