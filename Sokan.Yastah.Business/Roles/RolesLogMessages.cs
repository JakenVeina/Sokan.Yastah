using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Roles
{
    internal static class RolesLogMessages
    {
        public static void RoleUpdateNoChangesGiven(
                ILogger logger,
                long roleId)
            => _roleUpdateNoChangesGiven.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleUpdateNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(RoleUpdateNoChangesGiven)),
                    "Role update failed: No changes given:\r\n\tRoleId: {RoleId}")
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
                    new EventId(2002, nameof(RoleUpdateFailed)),
                    "Role update failed:\r\n\tRoleId: {RoleId}\r\n\tUpdateResult: {UpdateResult}")
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
                    new EventId(2003, nameof(RoleDeleteFailed)),
                    "Role delete failed:\r\n\tRoleId: {RoleId}\r\n\tDeleteResult: {DeleteResult}")
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
                    new EventId(2004, nameof(RoleIdsValidationFailed)),
                    "Role IDs validation failed:\r\n\tInvalidRoleIds: {InvalidRoleIds}")
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
                    new EventId(2005, nameof(RoleNameValidationFailed)),
                    "Role Name validation failed:\r\n\tRoleName: {RoleName}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

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
                    new EventId(2006, nameof(PermissionIdsValidationFailed)),
                    "Permission IDs validation failed:\r\n\tPermissionIds: {PermissionIds}\r\n\tValidationResult: {ValidationResult}")
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
                    new EventId(4001, nameof(RoleCreating)),
                    "Creating Role:\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4002, nameof(RoleCreated)),
                    "Role created:\r\n\tRoleId: {RoleId}")
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
                    new EventId(4003, nameof(RoleUpdating)),
                    "Updating Role:\r\n\tRoleId: {RoleId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4004, nameof(RoleUpdated)),
                    "Role updated:\r\n\tRoleId: {RoleId}\r\n\tUpdateResult: {UpdateResult}")
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
                    new EventId(4005, nameof(RoleDeleting)),
                    "Deleting Role:\r\n\tRoleId: {RoleId}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4006, nameof(RoleDeleted)),
                    "Role deleted:\r\n\tRoleId: {RoleId}\r\n\tVersionId: {VersionId}")
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
                    new EventId(4007, nameof(RolePermissionMappingsCreating)),
                    "Creating RolePermissionMapping range:\r\n\tRoleId: {RoleId}\r\n\tPermissionIds: {PermissionIds}")
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
                    new EventId(4008, nameof(RolePermissionMappingsCreated)),
                    "RolePermissionMapping range created:\r\n\tRoleId: {RoleId}\r\n\tMappingIds: {MappingIds}")
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
                    new EventId(4009, nameof(RolePermissionMappingsDeleting)),
                    "Deleting RolePermissionMapping:\r\n\tRoleId: {RoleId}\r\n\tMappingIds: {MappingIds}")
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
                    new EventId(4010, nameof(RolePermissionMappingsDeleted)),
                    "RolePermissionMapping deleted:\r\n\tRoleId: {RoleId}")
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
                    new EventId(4011, nameof(RoleUpdatingNotificationPublishing)),
                    $"Publishing {nameof(RoleUpdatingNotification)}:\r\n\tRoleId: {{RoleId}}")
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
                    new EventId(4012, nameof(RoleUpdatingNotificationPublished)),
                    $"{nameof(RoleUpdatingNotification)} published:\r\n\tRoleId: {{RoleId}}")
                .WithoutException();

        public static void RoleIdentitiesFetchingCurrent(
                ILogger logger)
            => _roleIdentitiesFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4013, nameof(RoleIdentitiesFetchingCurrent)),
                    "Fetching Role Identities")
                .WithoutException();

        public static void RoleIdentitiesFetchedCurrent(
                ILogger logger)
            => _roleIdentitiesFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4014, nameof(RoleIdentitiesFetchedCurrent)),
                    "Role Identities fetched")
                .WithoutException();

        public static void RoleIdentitiesCacheCleared(
                ILogger logger)
            => _roleIdentitiesCacheCleared.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdentitiesCacheCleared
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4015, nameof(RoleIdentitiesCacheCleared)),
                    "Role Identities cache cleared")
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
                    new EventId(4016, nameof(RolePermissionMappingIdentitiesFetched)),
                    "RolePermissionMapping Identities fetched:\r\n\tRoleId: {RoleId}")
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
                    new EventId(4017, nameof(RoleIdsValidating)),
                    "Role IDs validation succeded:\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void RoleIdsValidationSucceeded(
                ILogger logger)
            => _roleIdsValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdsValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4018, nameof(RoleIdsValidationSucceeded)),
                    "Role IDs validation succeded")
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
                    new EventId(4019, nameof(RoleNameValidationSucceeded)),
                    "Role Name validation succeded:\r\n\tRoleName: {RoleName}")
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
                    new EventId(4020, nameof(PermissionIdsValidationSucceeded)),
                    "Permission IDs validation succeded:\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();
    }
}
