using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Authorization
{
    internal static class AuthorizationLogMessages
    {
        public static void AuthenticatedUserNotFound(
                ILogger logger)
            => _authenticatedUserNotFound.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticatedUserNotFound
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(2001, nameof(AuthenticatedUserNotFound)),
                    "An authenticated user was required, but not found")
                .WithoutException();

        public static void RequiredPermissionsNotFound(
                ILogger logger,
                IReadOnlyCollection<int> missingPermissionIds)
            => _requiredPermissionsNotFound.Invoke(
                logger,
                missingPermissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _requiredPermissionsNotFound
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Warning,
                    new EventId(2002, nameof(RequiredPermissionsNotFound)),
                    "Some permissions were required but not found: MissingPermissionIds: {MissingPermissionIds}")
                .WithoutException();

        public static void AuthenticationRequired(
                ILogger logger)
            => _authenticationRequired.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationRequired
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(4001, nameof(AuthenticationRequired)),
                    "Checking for an authenticated user")
                .WithoutException();

        public static void AuthenticatedUserFound(
                ILogger logger,
                ulong userId)
            => _authenticatedUserFound.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _authenticatedUserFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(AuthenticatedUserFound)),
                    "An Authenticated user was required, and found: {UserId}")
                .WithoutException();

        public static void PermissionsRequired(
                ILogger logger,
                IReadOnlyCollection<int> permissionIds)
            => _permissionsRequired.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IReadOnlyCollection<int>> _permissionsRequired
            = LoggerMessage.Define<IReadOnlyCollection<int>>(
                    LogLevel.Warning,
                    new EventId(4003, nameof(PermissionsRequired)),
                    "Checking for required permissions: {PermissionIds}")
                .WithoutException();

        public static void RequiredPermissionsFound(
                ILogger logger)
            => _requiredPermissionsFound.Invoke(
                logger);
        private static readonly Action<ILogger> _requiredPermissionsFound
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(4004, nameof(RequiredPermissionsFound)),
                    "All required permissions found")
                .WithoutException();

        public static void MissingPermissionsFetched(
                ILogger logger,
                IReadOnlyDictionary<int, string> missingPermissions)
            => _missingPermissionsFetched.Invoke(
                logger,
                missingPermissions);
        private static readonly Action<ILogger, IReadOnlyDictionary<int, string>> _missingPermissionsFetched
            = LoggerMessage.Define<IReadOnlyDictionary<int, string>>(
                    LogLevel.Warning,
                    new EventId(4003, nameof(MissingPermissionsFetched)),
                    "Missing permissions fetched: {MissingPermissions}")
                .WithoutException();
    }
}
