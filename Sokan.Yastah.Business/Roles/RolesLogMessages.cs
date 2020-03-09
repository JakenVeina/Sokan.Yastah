using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Roles
{
    internal static class RolesLogMessages
    {
        public enum EventType
        {
            RoleCreating                            = BusinessLogEventType.Roles + 0x0001,
            RoleCreated                             = BusinessLogEventType.Roles + 0x0002,
            RoleNameValidationFailed                = BusinessLogEventType.Roles + 0x0003,
            RoleNameValidationSucceeded             = BusinessLogEventType.Roles + 0x0004,
            PermissionIdsValidationFailed           = BusinessLogEventType.Roles + 0x0005,
            PermissionIdsValidationSucceeded        = BusinessLogEventType.Roles + 0x0006,
            RoleDeleting                            = BusinessLogEventType.Roles + 0x0007,
            RoleDeleteFailed                        = BusinessLogEventType.Roles + 0x0008,
            RoleDeleted                             = BusinessLogEventType.Roles + 0x0009,
            RoleIdentitiesFetchingCurrent           = BusinessLogEventType.Roles + 0x000A,
            RoleIdentitiesFetchedCurrent            = BusinessLogEventType.Roles + 0x000B,
            RoleIdentitiesCacheCleared              = BusinessLogEventType.Roles + 0x000C,
            RoleUpdating                            = BusinessLogEventType.Roles + 0x000D,
            RoleUpdateNoChangesGiven                = BusinessLogEventType.Roles + 0x000E,
            RoleUpdateFailed                        = BusinessLogEventType.Roles + 0x000F,
            RoleUpdated                             = BusinessLogEventType.Roles + 0x0010,
            RoleUpdatingNotificationPublishing      = BusinessLogEventType.Roles + 0x0011,
            RoleUpdatingNotificationPublished       = BusinessLogEventType.Roles + 0x0012,
            RolePermissionMappingIdentitiesFetching = BusinessLogEventType.Roles + 0x0013,
            RolePermissionMappingIdentitiesFetched  = BusinessLogEventType.Roles + 0x0014,
            RolePermissionMappingsCreating          = BusinessLogEventType.Roles + 0x0015,
            RolePermissionMappingsCreated           = BusinessLogEventType.Roles + 0x0016,
            RolePermissionMappingsDeleting          = BusinessLogEventType.Roles + 0x0017,
            RolePermissionMappingsDeleted           = BusinessLogEventType.Roles + 0x0018,
            RoleIdsValidating                       = BusinessLogEventType.Roles + 0x0019,
            RoleIdsValidationFailed                 = BusinessLogEventType.Roles + 0x001A,
            RoleIdsValidationSucceeded              = BusinessLogEventType.Roles + 0x001B
        }

        public static void PermissionIdsValidationFailed(
                ILogger logger,
                IEnumerable<int> permissionIds,
                OperationResult validationResult)
            => _permissionIdsValidationFailed.Invoke(
                logger,
                permissionIds,
                validationResult);
        private static readonly Action<ILogger, IEnumerable<int>, OperationResult> _permissionIdsValidationFailed
            = LoggerMessage.Define<IEnumerable<int>, OperationResult>(
                    LogLevel.Warning,
                    EventType.PermissionIdsValidationFailed.ToEventId(),
                    "Permission IDs validation failed:\r\n\tPermissionIds: {PermissionIds}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void PermissionIdsValidationSucceeded(
                ILogger logger,
                IEnumerable<int> permissionIds)
            => _permissionIdsValidationSucceeded.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IEnumerable<int>> _permissionIdsValidationSucceeded
            = LoggerMessage.Define<IEnumerable<int>>(
                    LogLevel.Debug,
                    EventType.PermissionIdsValidationSucceeded.ToEventId(),
                    "Permission IDs validation succeded:\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();

        public static void RoleCreated(
                ILogger logger,
                long roleId)
            => _roleCreated.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RoleCreated.ToEventId(),
                    "Role created:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RoleCreating(
                ILogger logger,
                RoleCreationModel creationModel,
                ulong performedById)
            => _roleCreating.Invoke(
                logger,
                creationModel,
                performedById);
        private static readonly Action<ILogger, RoleCreationModel, ulong> _roleCreating
            = LoggerMessage.Define<RoleCreationModel, ulong>(
                    LogLevel.Debug,
                    EventType.RoleCreating.ToEventId(),
                    "Creating Role:\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void RoleDeleted(
                ILogger logger,
                long roleId,
                long versionId)
            => _roleDeleted.Invoke(
                logger,
                roleId,
                versionId);
        private static readonly Action<ILogger, long, long> _roleDeleted
            = LoggerMessage.Define<long, long>(
                    LogLevel.Debug,
                    EventType.RoleDeleted.ToEventId(),
                    "Role deleted:\r\n\tRoleId: {RoleId}\r\n\tVersionId: {VersionId}")
                .WithoutException();

        public static void RoleDeleteFailed(
                ILogger logger,
                long roleId,
                OperationResult updateResult)
            => _roleDeleteFailed.Invoke(
                logger,
                roleId,
                updateResult);
        private static readonly Action<ILogger, long, OperationResult> _roleDeleteFailed
            = LoggerMessage.Define<long, OperationResult>(
                    LogLevel.Warning,
                    EventType.RoleDeleteFailed.ToEventId(),
                    "Role delete failed:\r\n\tRoleId: {RoleId}\r\n\tDeleteResult: {DeleteResult}")
                .WithoutException();

        public static void RoleDeleting(
                ILogger logger,
                long roleId,
                ulong performedById)
            => _roleDeleting.Invoke(
                logger,
                roleId,
                performedById);
        private static readonly Action<ILogger, long, ulong> _roleDeleting
            = LoggerMessage.Define<long, ulong>(
                    LogLevel.Debug,
                    EventType.RoleDeleting.ToEventId(),
                    "Deleting Role:\r\n\tRoleId: {RoleId}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void RoleIdentitiesCacheCleared(
                ILogger logger)
            => _roleIdentitiesCacheCleared.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesCacheCleared
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.RoleIdentitiesCacheCleared.ToEventId(),
                    "Role Identities cache cleared")
                .WithoutException();

        public static void RoleIdentitiesFetchedCurrent(
                ILogger logger)
            => _roleIdentitiesFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.RoleIdentitiesFetchedCurrent.ToEventId(),
                    "Role Identities fetched")
                .WithoutException();

        public static void RoleIdentitiesFetchingCurrent(
                ILogger logger)
            => _roleIdentitiesFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.RoleIdentitiesFetchingCurrent.ToEventId(),
                    "Fetching Role Identities")
                .WithoutException();

        public static void RoleIdsValidating(
                ILogger logger,
                IEnumerable<long> roleIds)
            => _roleIdsValidating.Invoke(
                logger,
                roleIds);
        private static readonly Action<ILogger, IEnumerable<long>> _roleIdsValidating
            = LoggerMessage.Define<IEnumerable<long>>(
                    LogLevel.Debug,
                    EventType.RoleIdsValidating.ToEventId(),
                    "Role IDs validation succeded:\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void RoleIdsValidationFailed(
                ILogger logger,
                IEnumerable<long> invalidRoleIds)
            => _roleIdsValidationFailed.Invoke(
                logger,
                invalidRoleIds);
        private static readonly Action<ILogger, IEnumerable<long>> _roleIdsValidationFailed
            = LoggerMessage.Define<IEnumerable<long>>(
                    LogLevel.Warning,
                    EventType.RoleIdsValidationFailed.ToEventId(),
                    "Role IDs validation failed:\r\n\tInvalidRoleIds: {InvalidRoleIds}")
                .WithoutException();

        public static void RoleIdsValidationSucceeded(
                ILogger logger)
            => _roleIdsValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdsValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.RoleIdsValidationSucceeded.ToEventId(),
                    "Role IDs validation succeded")
                .WithoutException();

        public static void RoleNameValidationFailed(
                ILogger logger,
                string roleName,
                OperationResult validationResult)
            => _roleNameValidationFailed.Invoke(
                logger,
                roleName,
                validationResult);
        private static readonly Action<ILogger, string, OperationResult> _roleNameValidationFailed
            = LoggerMessage.Define<string, OperationResult>(
                    LogLevel.Warning,
                    EventType.RoleNameValidationFailed.ToEventId(),
                    "Role Name validation failed:\r\n\tRoleName: {RoleName}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void RoleNameValidationSucceeded(
                ILogger logger,
                string roleName)
            => _roleNameValidationSucceeded.Invoke(
                logger,
                roleName);
        private static readonly Action<ILogger, string> _roleNameValidationSucceeded
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    EventType.RoleNameValidationSucceeded.ToEventId(),
                    "Role Name validation succeded:\r\n\tRoleName: {RoleName}")
                .WithoutException();

        public static void RolePermissionMappingIdentitiesFetched(
                ILogger logger,
                long roleId)
            => _rolePermissionMappingIdentitiesFetched.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _rolePermissionMappingIdentitiesFetched
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingIdentitiesFetched.ToEventId(),
                    "RolePermissionMapping Identities fetched:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RolePermissionMappingIdentitiesFetching(
                ILogger logger,
                long roleId)
            => _rolePermissionMappingIdentitiesFetching.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _rolePermissionMappingIdentitiesFetching
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingIdentitiesFetching.ToEventId(),
                    "Fetching RolePermissionMapping Identities:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RolePermissionMappingsCreated(
                ILogger logger,
                long roleId,
                IEnumerable<long> mappingIds)
            => _rolePermissionMappingsCreated.Invoke(
                logger,
                roleId,
                mappingIds);
        private static readonly Action<ILogger, long, IEnumerable<long>> _rolePermissionMappingsCreated
            = LoggerMessage.Define<long, IEnumerable<long>>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingsCreated.ToEventId(),
                    "RolePermissionMapping range created:\r\n\tRoleId: {RoleId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void RolePermissionMappingsCreating(
                ILogger logger,
                long roleId,
                IEnumerable<int> permissionIds)
            => _rolePermissionMappingsCreating.Invoke(
                logger,
                roleId,
                permissionIds);
        private static readonly Action<ILogger, long, IEnumerable<int>> _rolePermissionMappingsCreating
            = LoggerMessage.Define<long, IEnumerable<int>>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingsCreating.ToEventId(),
                    "Creating RolePermissionMapping range:\r\n\tRoleId: {RoleId}\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();

        public static void RolePermissionMappingsDeleted(
                ILogger logger,
                long roleId)
            => _rolePermissionMappingsDeleted.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _rolePermissionMappingsDeleted
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingsDeleted.ToEventId(),
                    "RolePermissionMapping deleted:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RolePermissionMappingsDeleting(
                ILogger logger,
                long roleId,
                IEnumerable<long> mappingIds)
            => _rolePermissionMappingsDeleting.Invoke(
                logger,
                roleId,
                mappingIds);
        private static readonly Action<ILogger, long, IEnumerable<long>> _rolePermissionMappingsDeleting
            = LoggerMessage.Define<long, IEnumerable<long>>(
                    LogLevel.Debug,
                    EventType.RolePermissionMappingsDeleting.ToEventId(),
                    "Deleting RolePermissionMapping:\r\n\tRoleId: {RoleId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void RoleUpdated(
                ILogger logger,
                long roleId,
                OperationResult<long> updateResult)
            => _roleUpdated.Invoke(
                logger,
                roleId,
                updateResult);
        private static readonly Action<ILogger, long, OperationResult<long>> _roleUpdated
            = LoggerMessage.Define<long, OperationResult<long>>(
                    LogLevel.Debug,
                    EventType.RoleUpdated.ToEventId(),
                    "Role updated:\r\n\tRoleId: {RoleId}\r\n\tUpdateResult: {UpdateResult}")
                .WithoutException();

        public static void RoleUpdateFailed(
                ILogger logger,
                long roleId,
                OperationResult updateResult)
            => _roleUpdateFailed.Invoke(
                logger,
                roleId,
                updateResult);
        private static readonly Action<ILogger, long, OperationResult> _roleUpdateFailed
            = LoggerMessage.Define<long, OperationResult>(
                    LogLevel.Warning,
                    EventType.RoleUpdateFailed.ToEventId(),
                    "Role update failed:\r\n\tRoleId: {RoleId}\r\n\tUpdateResult: {UpdateResult}")
                .WithoutException();

        public static void RoleUpdateNoChangesGiven(
                ILogger logger,
                long roleId)
            => _roleUpdateNoChangesGiven.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleUpdateNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    EventType.RoleUpdateNoChangesGiven.ToEventId(),
                    "Role update failed: No changes given:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RoleUpdating(
                ILogger logger,
                long roleId,
                RoleUpdateModel updateModel,
                ulong performedById)
            => _roleUpdating.Invoke(
                logger,
                roleId,
                updateModel,
                performedById);
        private static readonly Action<ILogger, long, RoleUpdateModel, ulong> _roleUpdating
            = LoggerMessage.Define<long, RoleUpdateModel, ulong>(
                    LogLevel.Debug,
                    EventType.RoleUpdating.ToEventId(),
                    "Updating Role:\r\n\tRoleId: {RoleId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void RoleUpdatingNotificationPublished(
                ILogger logger,
                long roleId)
            => _roleUpdatingNotificationPublished.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleUpdatingNotificationPublished
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RoleUpdatingNotificationPublished.ToEventId(),
                    $"{nameof(RoleUpdatingNotification)} published:\r\n\tRoleId: {{RoleId}}")
                .WithoutException();

        public static void RoleUpdatingNotificationPublishing(
                ILogger logger,
                long roleId)
            => _roleUpdatingNotificationPublishing.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleUpdatingNotificationPublishing
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RoleUpdatingNotificationPublishing.ToEventId(),
                    $"Publishing {nameof(RoleUpdatingNotification)}:\r\n\tRoleId: {{RoleId}}")
                .WithoutException();
    }
}
