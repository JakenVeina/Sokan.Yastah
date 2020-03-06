using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Permissions
{
    internal static class PermissionsLogMessages
    {
        public static void PermissionIdsValidationFailed(
                ILogger logger,
                IReadOnlyCollection<int> invalidPermissionIds)
            => _permissionIdsValidationFailed.Invoke(
                logger,
                invalidPermissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _permissionIdsValidationFailed
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(PermissionIdsValidationFailed)),
                    "Permission IDs were invalid: {InvalidPermissionIds}")
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
                    new EventId(4001, nameof(PermissionIdsValidationFailed)),
                    "Validating Permission IDs: {PermissionIds}")
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
                    new EventId(4002, nameof(PermissionIdsValidationSucceeded)),
                    "Permission IDs validated: {PermisionIds}")
                .WithoutException();

        public static void PermissionCategoryDescriptionsFetching(
                ILogger logger)
            => _permissionCategoryDescriptionsFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionCategoryDescriptionsFetching
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(4003, nameof(PermissionCategoryDescriptionsFetching)),
                    "Fetching PermissionCategory Descriptions")
                .WithoutException();

        public static void PermissionCategoryDescriptionsFetched(
                ILogger logger)
            => _permissionCategoryDescriptionsFetched.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionCategoryDescriptionsFetched
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(4004, nameof(PermissionCategoryDescriptionsFetched)),
                    "PermissionCategory Descriptions fetched")
                .WithoutException();

        public static void PermissionIdentitiesFetching(
                ILogger logger)
            => _permissionIdentitiesFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesFetching
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(4005, nameof(PermissionIdentitiesFetching)),
                    "Fetching Permission Identities")
                .WithoutException();

        public static void PermissionIdentitiesFetched(
                ILogger logger)
            => _permissionIdentitiesFetched.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdentitiesFetched
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(4006, nameof(PermissionIdentitiesFetched)),
                    "Permission Identities fetched")
                .WithoutException();
    }
}
