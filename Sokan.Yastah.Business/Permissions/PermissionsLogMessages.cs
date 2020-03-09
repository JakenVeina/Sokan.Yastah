using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Permissions
{
    internal static class PermissionsLogMessages
    {
        public enum EventType
        {
            PermissionCategoryDescriptionsFetching  = BusinessLogEventType.Permissions + 0x0001,
            PermissionCategoryDescriptionsFetched   = BusinessLogEventType.Permissions + 0x0002,
            PermissionIdentitiesFetching            = BusinessLogEventType.Permissions + 0x0003,
            PermissionIdentitiesFetched             = BusinessLogEventType.Permissions + 0x0004,
            PermissionIdsValidating                 = BusinessLogEventType.Permissions + 0x0005,
            PermissionIdsValidationFailed           = BusinessLogEventType.Permissions + 0x0006,
            PermissionIdsValidationSucceeded        = BusinessLogEventType.Permissions + 0x0007
        }

        public static void PermissionCategoryDescriptionsFetched(
                ILogger logger)
            => _permissionCategoryDescriptionsFetched.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionCategoryDescriptionsFetched
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.PermissionCategoryDescriptionsFetched.ToEventId(),
                    "PermissionCategory Descriptions fetched")
                .WithoutException();

        public static void PermissionCategoryDescriptionsFetching(
                ILogger logger)
            => _permissionCategoryDescriptionsFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionCategoryDescriptionsFetching
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.PermissionCategoryDescriptionsFetching.ToEventId(),
                    "Fetching PermissionCategory Descriptions")
                .WithoutException();

        public static void PermissionIdentitiesFetched(
                ILogger logger)
            => _permissionIdentitiesFetched.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesFetched
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.PermissionIdentitiesFetched.ToEventId(),
                    "Permission Identities fetched")
                .WithoutException();

        public static void PermissionIdentitiesFetching(
                ILogger logger)
            => _permissionIdentitiesFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesFetching
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.PermissionIdentitiesFetching.ToEventId(),
                    "Fetching Permission Identities")
                .WithoutException();

        public static void PermissionIdsValidating(
                ILogger logger,
                IReadOnlyCollection<int> permissionIds)
            => _permissionIdsValidating.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _permissionIdsValidating
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Information,
                    EventType.PermissionIdsValidationFailed.ToEventId(),
                    "Validating Permission IDs: {PermissionIds}")
                .WithoutException();

        public static void PermissionIdsValidationFailed(
                ILogger logger,
                IReadOnlyCollection<int> invalidPermissionIds)
            => _permissionIdsValidationFailed.Invoke(
                logger,
                invalidPermissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _permissionIdsValidationFailed
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Warning,
                    EventType.PermissionIdsValidationFailed.ToEventId(),
                    "Permission IDs were invalid: {InvalidPermissionIds}")
                .WithoutException();

        public static void PermissionIdsValidationSucceeded(
                ILogger logger,
                IReadOnlyCollection<int> permissionIds)
            => _permissionIdsValidationSucceeded.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _permissionIdsValidationSucceeded
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Information,
                    EventType.PermissionIdsValidationSucceeded.ToEventId(),
                    "Permission IDs validated: {PermisionIds}")
                .WithoutException();
    }
}
