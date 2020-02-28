using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Roles
{
    public static class RolesLogMessages
    {
        public static void RoleVersionNotFound(
                ILogger logger,
                long roleId)
            => _roleVersionNotFound.Invoke(
                logger,
                roleId);
        private static Action<ILogger, long> _roleVersionNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(RoleVersionNotFound)),
                    $"{nameof(RoleVersionEntity)} not found: {{RoleId}}")
                .WithoutException();

        public static void RoleNoChangesGiven(
                ILogger logger,
                long roleId)
            => _roleNoChangesGiven.Invoke(
                logger,
                roleId);
        private static Action<ILogger, long> _roleNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    new EventId(2002, nameof(RoleNoChangesGiven)),
                    $"No changes given for {nameof(RoleEntity)}: {{RoleId}}")
                .WithoutException();

        public static void RoleCreating(
                ILogger logger,
                string name,
                long actionId)
            => _roleCreating.Invoke(
                logger,
                name,
                actionId);
        private static Action<ILogger, string, long> _roleCreating
            = LoggerMessage.Define<string, long>(
                    LogLevel.Information,
                    new EventId(3001, nameof(RoleCreating)),
                    $"Creating {nameof(RoleEntity)}:\r\n\tName: {{Name}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void RoleCreated(
                ILogger logger,
                long roleId,
                long versionId)
            => _roleCreated.Invoke(
                logger,
                roleId,
                versionId);
        private static Action<ILogger, long, long> _roleCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    new EventId(3002, nameof(RoleCreated)),
                    $"{nameof(RoleEntity)} created: {{RoleId}}, version {{VersionId}}")
                .WithoutException();

        public static void RolePermissionMappingsCreating(
                ILogger logger,
                long roleId,
                IEnumerable<int> permissionIds,
                long actionId)
            => _rolePermissionMappingsCreating.Invoke(
                logger,
                roleId,
                permissionIds,
                actionId);
        private static Action<ILogger, long, IEnumerable<int>, long> _rolePermissionMappingsCreating
            = LoggerMessage.Define<long, IEnumerable<int>, long>(
                    LogLevel.Information,
                    new EventId(3003, nameof(RolePermissionMappingsCreating)),
                    $"Creating {nameof(RolePermissionMappingEntity)} Range:\r\n\tRoleId: {{RoleId}}\r\n\tPermissionIds: {{PermissionIds}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void RolePermissionMappingCreating(
                ILogger logger,
                long roleId,
                int permissionId,
                long actionId)
            => _rolePermissionMappingCreating.Invoke(
                logger,
                roleId,
                permissionId,
                actionId);
        private static Action<ILogger, long, int, long> _rolePermissionMappingCreating
            = LoggerMessage.Define<long, int, long>(
                    LogLevel.Information,
                    new EventId(3004, nameof(RolePermissionMappingCreating)),
                    $"Creating {nameof(RolePermissionMappingEntity)}:\r\n\tRoleId: {{RoleId}}\r\n\tPermissionId: {{PermissionId}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void RolePermissionMappingCreated(
                ILogger logger,
                long mappingId)
            => _rolePermissionMappingCreated.Invoke(
                logger,
                mappingId);
        private static Action<ILogger, long> _rolePermissionMappingCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3005, nameof(RolePermissionMappingCreated)),
                    $"{nameof(RolePermissionMappingEntity)} created: {{MappingId}}")
                .WithoutException();

        public static void RolePermissionMappingsCreated(
                ILogger logger)
            => _rolePermissionMappingsCreated.Invoke(
                logger);
        private static Action<ILogger> _rolePermissionMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3006, nameof(RolePermissionMappingsCreated)),
                    $"{nameof(RolePermissionMappingEntity)} range created")
                .WithoutException();

        public static void RoleUpdating(
                ILogger logger,
                long roleId,
                long actionId,
                Optional<string> name = default,
                Optional<bool> isDeleted = default)
            => _roleUpdating.Invoke(
                logger,
                roleId,
                actionId,
                name,
                isDeleted);
        private static Action<ILogger, long, long, Optional<string>, Optional<bool>> _roleUpdating
            = LoggerMessage.Define<long, long, Optional<string>, Optional<bool>>(
                    LogLevel.Information,
                    new EventId(3007, nameof(RoleUpdating)),
                    $"Updating {nameof(RoleEntity)}:\r\n\tRoleId: {{RoleId}}\r\n\tActionId: {{ActionId}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleUpdated(
                ILogger logger,
                long mappingId,
                long versionId)
            => _roleUpdated.Invoke(
                logger,
                mappingId,
                versionId);
        private static Action<ILogger, long, long> _roleUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    new EventId(3008, nameof(RoleUpdated)),
                    $"{nameof(RoleEntity)} updated:\r\n\tMappingId: {{MappingId}}\r\n\tVersionId: {{VersionId}}")
                .WithoutException();

        public static void RolePermissionMappingsUpdating(
                ILogger logger,
                IEnumerable<long> mappingIds,
                long deletionId)
            => _rolePermissionMappingsUpdating.Invoke(
                logger,
                mappingIds,
                deletionId);
        private static Action<ILogger, IEnumerable<long>, long> _rolePermissionMappingsUpdating
            = LoggerMessage.Define<IEnumerable<long>, long>(
                    LogLevel.Information,
                    new EventId(3009, nameof(RolePermissionMappingsUpdating)),
                    $"Updating {nameof(RolePermissionMappingEntity)} Range:\r\n\tMappingIds: {{MappingIds}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void RolePermissionMappingUpdating(
                ILogger logger,
                long mappingId,
                long deletionId)
            => _rolePermissionMappingUpdating.Invoke(
                logger,
                mappingId,
                deletionId);
        private static Action<ILogger, long, long> _rolePermissionMappingUpdating
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    new EventId(3010, nameof(RolePermissionMappingUpdating)),
                    $"Updating {nameof(RolePermissionMappingEntity)}:\r\n\tMappingIds: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void RolePermissionMappingsUpdated(
                ILogger logger)
            => _rolePermissionMappingsUpdated.Invoke(
                logger);
        private static Action<ILogger> _rolePermissionMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3011, nameof(RolePermissionMappingsUpdated)),
                    $"{nameof(RolePermissionMappingEntity)} updated")
                .WithoutException();

        public static void RoleVersionsEnumeratingAny(
                ILogger logger,
                Optional<IEnumerable<long>> excludedRoleIds,
                Optional<string> name,
                Optional<bool> isDeleted,
                Optional<bool> isLatestVersion)
            => _roleVersionsEnumeratingAny.Invoke(
                logger,
                excludedRoleIds,
                name,
                isDeleted,
                isLatestVersion);
        private static readonly Action<ILogger, Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>> _roleVersionsEnumeratingAny
            = LoggerMessage.Define<Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(RoleVersionsEnumeratingAny)),
                    $"Enumerating for any {nameof(RoleVersionEntity)}:\r\n\tExcludedRoleIds: {{ExcludedRoleIds}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}\r\n\tIsLatestVersion: {{IsLatestVersion}}")
                .WithoutException();

        public static void RoleIdentitiesEnumerating(
                ILogger logger,
                Optional<bool> isDeleted)
            => _roleIdentitiesEnumerating.Invoke(
                logger,
                isDeleted);
        private static readonly Action<ILogger,  Optional<bool>> _roleIdentitiesEnumerating
            = LoggerMessage.Define<Optional<bool>>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(RoleIdentitiesEnumerating)),
                    $"Enumerating {nameof(RoleIdentityViewModel)}:\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RolePermissionMappingIdentitiesEnumerating(
                ILogger logger,
                long roleId,
                Optional<bool> isDeleted)
            => _rolePermissionMappingIdentitiesEnumerating.Invoke(
                logger,
                roleId,
                isDeleted);
        private static readonly Action<ILogger, long, Optional<bool>> _rolePermissionMappingIdentitiesEnumerating
            = LoggerMessage.Define<long, Optional<bool>>(
                    LogLevel.Debug,
                    new EventId(4004, nameof(RolePermissionMappingIdentitiesEnumerating)),
                    $"Enumerating {nameof(RoleIdentityViewModel)}:\r\n\tRoleId: {{RoleId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleDetailReading(
                ILogger logger,
                long roleId,
                Optional<bool> isDeleted)
            => _roleDetailReading.Invoke(
                logger,
                roleId,
                isDeleted);
        private static readonly Action<ILogger, long, Optional<bool>> _roleDetailReading
            = LoggerMessage.Define<long, Optional<bool>>(
                    LogLevel.Debug,
                    new EventId(4005, nameof(RoleDetailReading)),
                    $"Reading {nameof(RoleDetailViewModel)}:\r\n\tRoleId: {{RoleId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleDetailRead(
                ILogger logger,
                long roleId)
            => _roleDetailRead.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleDetailRead
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4006, nameof(RoleDetailRead)),
                    $"{nameof(RoleDetailViewModel)} read: {{RoleId}}")
                .WithoutException();
    }
}
