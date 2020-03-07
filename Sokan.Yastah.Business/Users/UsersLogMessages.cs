using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Users
{
    internal static class UsersLogMessages
    {
        public static void UserNotFound(
                ILogger logger,
                ulong userId)
            => _userNotFound.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userNotFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(UserNotFound)),
                    "User not found: {UserId}")
                .WithoutException();

        public static void UserUpdateNoChangesGiven(
                ILogger logger,
                ulong userId)
            => _userUpdateNoChangesGiven.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userUpdateNoChangesGiven
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    new EventId(2002, nameof(UserUpdateNoChangesGiven)),
                    "User update failed: No changes given:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void PermissionIdsValidationFailed(
                ILogger logger,
                OperationResult validationResult)
            => _permissionIdsValidationFailed.Invoke(
                logger,
                validationResult);
        private static readonly Action<ILogger, OperationResult> _permissionIdsValidationFailed
            = LoggerMessage.Define<OperationResult>(
                    LogLevel.Warning,
                    new EventId(2003, nameof(PermissionIdsValidationFailed)),
                    "Permission IDs validation failed:\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void RoleIdsValidationFailed(
                ILogger logger,
                OperationResult validationResult)
            => _roleIdsValidationFailed.Invoke(
                logger,
                validationResult);
        private static readonly Action<ILogger, OperationResult> _roleIdsValidationFailed
            = LoggerMessage.Define<OperationResult>(
                    LogLevel.Warning,
                    new EventId(2004, nameof(RoleIdsValidationFailed)),
                    "Role IDs validation failed:\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void UserTracking(
                ILogger logger,
                ulong userId,
                string username,
                string discriminator,
                string avatarHash)
            => _userTracking.Invoke(
                logger,
                userId,
                username,
                discriminator,
                avatarHash);
        private static readonly Action<ILogger, ulong, string, string, string> _userTracking
            = LoggerMessage.Define<ulong, string, string, string>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(UserTracking)),
                    "Tracking User:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}\r\n\tAvatarHash: {AvatarHash}")
                .WithoutException();

        public static void UserTracked(
                ILogger logger,
                ulong userId)
            => _userTracked.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userTracked
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(UserUpdated)),
                    "User tracked:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserUpdating(
                ILogger logger,
                ulong userId,
                UserUpdateModel updateModel,
                ulong performedById)
            => _userUpdating.Invoke(
                logger,
                userId,
                updateModel,
                performedById);
        private static readonly Action<ILogger, ulong, UserUpdateModel, ulong> _userUpdating
            = LoggerMessage.Define<ulong, UserUpdateModel, ulong>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(UserUpdating)),
                    "Updating User:\r\n\tUserId: {UserId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void UserUpdated(
                ILogger logger,
                ulong userId)
            => _userUpdated.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userUpdated
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4004, nameof(UserUpdated)),
                    "User updated:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserPermissionMappingsCreating(
                ILogger logger,
                ulong userId,
                IEnumerable<int> permissionIds,
                PermissionMappingType mappingType)
            => _userPermissionMappingsCreating.Invoke(
                logger,
                userId,
                permissionIds,
                mappingType);
        private static readonly Action<ILogger, ulong, IEnumerable<int>, PermissionMappingType> _userPermissionMappingsCreating
            = LoggerMessage.Define<ulong, IEnumerable<int>, PermissionMappingType>(
                    LogLevel.Debug,
                    new EventId(4005, nameof(UserPermissionMappingsCreating)),
                    "Creating UserPermissionMapping range:\r\n\tUserId: {UserId}\r\n\tPermissionIds: {PermissionIds}\r\n\tMappingType: {MappingType}")
                .WithoutException();

        public static void UserPermissionMappingsCreated(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userPermissionMappingsCreated.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userPermissionMappingsCreated
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4006, nameof(UserPermissionMappingsCreated)),
                    "UserPermissionMapping range created:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserPermissionMappingsDeleting(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userPermissionMappingsDeleting.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userPermissionMappingsDeleting
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4007, nameof(UserPermissionMappingsDeleting)),
                    "Deleting UserPermissionMapping range:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserPermissionMappingsDeleted(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userPermissionMappingsDeleted.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userPermissionMappingsDeleted
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4008, nameof(UserPermissionMappingsDeleted)),
                    "UserPermissionMapping range deleted:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserRoleMappingsCreating(
                ILogger logger,
                ulong userId,
                IEnumerable<long> roleIds)
            => _userRoleMappingsCreating.Invoke(
                logger,
                userId,
                roleIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userRoleMappingsCreating
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4009, nameof(UserRoleMappingsCreating)),
                    "Creating UserRoleMapping range:\r\n\tUserId: {UserId}\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void UserRoleMappingsCreated(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userRoleMappingsCreated.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userRoleMappingsCreated
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4010, nameof(UserRoleMappingsCreated)),
                    "UserRoleMapping range created:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserRoleMappingsDeleting(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userRoleMappingsDeleting.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userRoleMappingsDeleting
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4011, nameof(UserRoleMappingsDeleting)),
                    "Deleting UserRoleMapping range:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserRoleMappingsDeleted(
                ILogger logger,
                ulong userId,
                IEnumerable<long> mappingIds)
            => _userRoleMappingsDeleted.Invoke(
                logger,
                userId,
                mappingIds);
        private static readonly Action<ILogger, ulong, IEnumerable<long>> _userRoleMappingsDeleted
            = LoggerMessage.Define<ulong, IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4012, nameof(UserRoleMappingsDeleted)),
                    "UserRoleMapping range deleted:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
                .WithoutException();

        public static void UserInitializingNotificationPublishing(
                ILogger logger,
                ulong userId)
            => _userInitializingNotificationPublishing.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userInitializingNotificationPublishing
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4013, nameof(UserInitializingNotificationPublishing)),
                    $"Publishing {nameof(UserInitializingNotification)}:\r\n\tUserId: {{UserId}}")
                .WithoutException();

        public static void UserInitializingNotificationPublished(
                ILogger logger,
                ulong userId)
            => _userInitializingNotificationPublished.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userInitializingNotificationPublished
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4014, nameof(UserInitializingNotificationPublished)),
                    $"{nameof(UserInitializingNotification)} published:\r\n\tUserId: {{UserId}}")
                .WithoutException();

        public static void UserUpdatingNotificationPublishing(
                ILogger logger,
                ulong userId)
            => _userUpdatingNotificationPublishing.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userUpdatingNotificationPublishing
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4015, nameof(UserUpdatingNotificationPublishing)),
                    $"Publishing {nameof(UserUpdatingNotification)}:\r\n\tUserId: {{UserId}}")
                .WithoutException();

        public static void UserUpdatingNotificationPublished(
                ILogger logger,
                ulong userId)
            => _userUpdatingNotificationPublished.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userUpdatingNotificationPublished
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4016, nameof(UserUpdatingNotificationPublished)),
                    $"{nameof(UserUpdatingNotification)} published:\r\n\tUserId: {{UserId}}")
                .WithoutException();

        public static void DefaultPermissionIdsFetched(
                ILogger logger,
                IEnumerable<int> permissionIds)
            => _defaultPermissionIdsFetched.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IEnumerable<int>> _defaultPermissionIdsFetched
            = LoggerMessage.Define<IEnumerable<int>>(
                    LogLevel.Debug,
                    new EventId(4017, nameof(DefaultPermissionIdsFetched)),
                    "DefaultPermissionMapping IDs fetched:\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();

        public static void DefaultRoleIdsFetched(
                ILogger logger,
                IEnumerable<long> roleIds)
            => _userRoleIdsFetched.Invoke(
                logger,
                roleIds);
        private static readonly Action<ILogger, IEnumerable<long>> _userRoleIdsFetched
            = LoggerMessage.Define<IEnumerable<long>>(
                    LogLevel.Debug,
                    new EventId(4018, nameof(DefaultRoleIdsFetched)),
                    "DefaultRoleMapping IDs fetched:\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void UserPermissionMappingIdentitiesFetched(
                ILogger logger,
                ulong userId)
            => _userPermissionMappingIdentitiesFetched.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userPermissionMappingIdentitiesFetched
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4019, nameof(UserPermissionMappingIdentitiesFetched)),
                    "UserPermissionMapping Identities fetched:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserRoleMappingIdentitiesFetched(
                ILogger logger,
                ulong userId)
            => _userRoleMappingIdentitiesFetched.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userRoleMappingIdentitiesFetched
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4021, nameof(UserRoleMappingIdentitiesFetched)),
                    "UserRoleMapping Identities fetched:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void GrantedPermissionIdentitiesFetching(
                ILogger logger,
                ulong userId)
            => _grantedPermissionIdentitiesFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _grantedPermissionIdentitiesFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4022, nameof(GrantedPermissionIdentitiesFetching)),
                    "Fetching Granted Permission IDs:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void GrantedPermissionIdentitiesFetched(
                ILogger logger,
                ulong userId)
            => _grantedPermissionIdentitiesFetched.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _grantedPermissionIdentitiesFetched
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4023, nameof(GrantedPermissionIdentitiesFetched)),
                    "Granted Permission IDs fetched:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void RoleMemberIdsFetching(
                ILogger logger,
                long roleId)
            => _roleMemberIdsFetching.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleMemberIdsFetching
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4019, nameof(RoleMemberIdsFetching)),
                    "Fetching Role member IDs:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void RoleMemberIdsFetched(
                ILogger logger,
                IEnumerable<ulong> memberIds)
            => _roleMemberIdsFetched.Invoke(
                logger,
                memberIds);
        private static readonly Action<ILogger, IEnumerable<ulong>> _roleMemberIdsFetched
            = LoggerMessage.Define<IEnumerable<ulong>>(
                    LogLevel.Debug,
                    new EventId(4020, nameof(RoleMemberIdsFetched)),
                    "Role member IDs fetched:\r\n\tMemberIds: {MemberIds}")
                .WithoutException();

        public static void RoleMemberIdsCacheCleared(
                ILogger logger,
                long roleId)
            => _roleMemberIdsCacheCleared.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleMemberIdsCacheCleared
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4020, nameof(RoleMemberIdsCacheCleared)),
                    "Role member IDs cache cleared:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void PermissionIdsValidating(
                ILogger logger,
                IEnumerable<int> permissionIds)
            => _permissionIdsValidating.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IEnumerable<int>> _permissionIdsValidating
            = LoggerMessage.Define<IEnumerable<int>>(
                    LogLevel.Debug,
                    new EventId(4021, nameof(PermissionIdsValidating)),
                    "Permission IDs validation succeded:\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();

        public static void PermissionIdsValidationSucceeded(
                ILogger logger)
            => _permissionIdsValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdsValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4022, nameof(PermissionIdsValidationSucceeded)),
                    "Permission IDs validation succeded")
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
                    new EventId(4023, nameof(RoleIdsValidating)),
                    "Role IDs validation succeded:\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void RoleIdsValidationSucceeded(
                ILogger logger)
            => _roleIdsValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _roleIdsValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4024, nameof(RoleIdsValidationSucceeded)),
                    "Role IDs validation succeded")
                .WithoutException();

        public static void UserIsAdmin(
                ILogger logger,
                ulong userId)
            => _userIsAdmin.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userIsAdmin
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4025, nameof(UserIsAdmin)),
                    "User is administrator: skipping permissions lookup:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserFound(
                ILogger logger,
                ulong userId)
            => _userFound.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4026, nameof(UserFound)),
                    "User found: {UserId}")
                .WithoutException();

        public static void UserCreated(
                ILogger logger,
                ulong userId)
            => _userCreated.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userCreated
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4027, nameof(UserCreated)),
                    "User created: {UserId}")
                .WithoutException();
    }
}
