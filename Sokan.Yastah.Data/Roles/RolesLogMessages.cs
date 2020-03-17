using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Roles
{
    public static class RolesLogMessages
    {
        public enum EventType
        {
            RoleVersionsEnumeratingAny                  = DataLogEventType.Roles + 0x0001,
            RoleIdentitiesEnumerating                   = DataLogEventType.Roles + 0x0002,
            RolePermissionMappingIdentitiesEnumerating  = DataLogEventType.Roles + 0x0003,
            RoleCreating                                = DataLogEventType.Roles + 0x0004,
            RoleCreated                                 = DataLogEventType.Roles + 0x0005,
            RolePermissionMappingsCreating              = DataLogEventType.Roles + 0x0006,
            RolePermissionMappingsCreated               = DataLogEventType.Roles + 0x0007,
            RolePermissionMappingCreating               = DataLogEventType.Roles + 0x0008,
            RolePermissionMappingCreated                = DataLogEventType.Roles + 0x0009,
            RoleDetailReading                           = DataLogEventType.Roles + 0x000A,
            RoleDetailRead                              = DataLogEventType.Roles + 0x000B,
            RoleVersionNotFound                         = DataLogEventType.Roles + 0x000C,
            RoleUpdating                                = DataLogEventType.Roles + 0x000D,
            RoleUpdated                                 = DataLogEventType.Roles + 0x000E,
            RoleNoChangesGiven                          = DataLogEventType.Roles + 0x000F,
            RolePermissionMappingsUpdating              = DataLogEventType.Roles + 0x0010,
            RolePermissionMappingsUpdated               = DataLogEventType.Roles + 0x0011,
            RolePermissionMappingUpdating               = DataLogEventType.Roles + 0x0012
        }

        public static void RoleCreated(
                ILogger logger,
                long roleId,
                long versionId)
            => _roleCreated.Invoke(
                logger,
                roleId,
                versionId);
        private static readonly Action<ILogger, long, long> _roleCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.RoleCreated.ToEventId(),
                    $"{nameof(RoleEntity)} created: {{RoleId}}, version {{VersionId}}")
                .WithoutException();

        public static void RoleCreating(
                ILogger logger,
                string name,
                long actionId)
            => _roleCreating.Invoke(
                logger,
                name,
                actionId);
        private static readonly Action<ILogger, string, long> _roleCreating
            = LoggerMessage.Define<string, long>(
                    LogLevel.Information,
                    EventType.RoleCreating.ToEventId(),
                    $"Creating {nameof(RoleEntity)}:\r\n\tName: {{Name}}\r\n\tActionId: {{ActionId}}")
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
                    EventType.RoleDetailRead.ToEventId(),
                    $"{nameof(RoleDetailViewModel)} read: {{RoleId}}")
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
                    EventType.RoleDetailReading.ToEventId(),
                    $"Reading {nameof(RoleDetailViewModel)}:\r\n\tRoleId: {{RoleId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleIdentitiesEnumerating(
                ILogger logger,
                Optional<bool> isDeleted)
            => _roleIdentitiesEnumerating.Invoke(
                logger,
                isDeleted);
        private static readonly Action<ILogger, Optional<bool>> _roleIdentitiesEnumerating
            = LoggerMessage.Define<Optional<bool>>(
                    LogLevel.Debug,
                    EventType.RoleIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(RoleIdentityViewModel)}:\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleNoChangesGiven(
                ILogger logger,
                long roleId)
            => _roleNoChangesGiven.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    EventType.RoleNoChangesGiven.ToEventId(),
                    $"No changes given for {nameof(RoleEntity)}: {{RoleId}}")
                .WithoutException();

        public static void RolePermissionMappingCreated(
                ILogger logger,
                long mappingId)
            => _rolePermissionMappingCreated.Invoke(
                logger,
                mappingId);
        private static readonly Action<ILogger, long> _rolePermissionMappingCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.RolePermissionMappingCreated.ToEventId(),
                    $"{nameof(RolePermissionMappingEntity)} created: {{MappingId}}")
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
        private static readonly Action<ILogger, long, int, long> _rolePermissionMappingCreating
            = LoggerMessage.Define<long, int, long>(
                    LogLevel.Information,
                    EventType.RolePermissionMappingCreating.ToEventId(),
                    $"Creating {nameof(RolePermissionMappingEntity)}:\r\n\tRoleId: {{RoleId}}\r\n\tPermissionId: {{PermissionId}}\r\n\tActionId: {{ActionId}}")
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
                    EventType.RolePermissionMappingIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(RoleIdentityViewModel)}:\r\n\tRoleId: {{RoleId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RolePermissionMappingsCreated(
                ILogger logger)
            => _rolePermissionMappingsCreated.Invoke(
                logger);
        private static readonly Action<ILogger> _rolePermissionMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.RolePermissionMappingsCreated.ToEventId(),
                    $"{nameof(RolePermissionMappingEntity)} range created")
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
        private static readonly Action<ILogger, long, IEnumerable<int>, long> _rolePermissionMappingsCreating
            = LoggerMessage.Define<long, IEnumerable<int>, long>(
                    LogLevel.Information,
                    EventType.RolePermissionMappingsCreating.ToEventId(),
                    $"Creating {nameof(RolePermissionMappingEntity)} Range:\r\n\tRoleId: {{RoleId}}\r\n\tPermissionIds: {{PermissionIds}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void RolePermissionMappingsUpdated(
                ILogger logger)
            => _rolePermissionMappingsUpdated.Invoke(
                logger);
        private static readonly Action<ILogger> _rolePermissionMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.RolePermissionMappingsUpdated.ToEventId(),
                    $"{nameof(RolePermissionMappingEntity)} updated")
                .WithoutException();

        public static void RolePermissionMappingUpdating(
                ILogger logger,
                long mappingId,
                long deletionId)
            => _rolePermissionMappingUpdating.Invoke(
                logger,
                mappingId,
                deletionId);
        private static readonly Action<ILogger, long, long> _rolePermissionMappingUpdating
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.RolePermissionMappingUpdating.ToEventId(),
                    $"Updating {nameof(RolePermissionMappingEntity)}:\r\n\tMappingIds: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void RoleUpdated(
                ILogger logger,
                long mappingId,
                long versionId)
            => _roleUpdated.Invoke(
                logger,
                mappingId,
                versionId);
        private static readonly Action<ILogger, long, long> _roleUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.RoleUpdated.ToEventId(),
                    $"{nameof(RoleEntity)} updated:\r\n\tMappingId: {{MappingId}}\r\n\tVersionId: {{VersionId}}")
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
        private static readonly Action<ILogger, long, long, Optional<string>, Optional<bool>> _roleUpdating
            = LoggerMessage.Define<long, long, Optional<string>, Optional<bool>>(
                    LogLevel.Information,
                    EventType.RoleUpdating.ToEventId(),
                    $"Updating {nameof(RoleEntity)}:\r\n\tRoleId: {{RoleId}}\r\n\tActionId: {{ActionId}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void RoleVersionNotFound(
                ILogger logger,
                long roleId)
            => _roleVersionNotFound.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleVersionNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    EventType.RoleVersionNotFound.ToEventId(),
                    $"{nameof(RoleVersionEntity)} not found: {{RoleId}}")
                .WithoutException();

        public static void RolePermissionMappingsUpdating(
                ILogger logger,
                IEnumerable<long> mappingIds,
                long deletionId)
            => _rolePermissionMappingsUpdating.Invoke(
                logger,
                mappingIds,
                deletionId);
        private static readonly Action<ILogger, IEnumerable<long>, long> _rolePermissionMappingsUpdating
            = LoggerMessage.Define<IEnumerable<long>, long>(
                    LogLevel.Information,
                    EventType.RolePermissionMappingsUpdating.ToEventId(),
                    $"Updating {nameof(RolePermissionMappingEntity)} Range:\r\n\tMappingIds: {{MappingIds}}\r\n\tDeletionId: {{DeletionId}}")
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
                    EventType.RoleVersionsEnumeratingAny.ToEventId(),
                    $"Enumerating for any {nameof(RoleVersionEntity)}:\r\n\tExcludedRoleIds: {{ExcludedRoleIds}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}\r\n\tIsLatestVersion: {{IsLatestVersion}}")
                .WithoutException();
    }
}
