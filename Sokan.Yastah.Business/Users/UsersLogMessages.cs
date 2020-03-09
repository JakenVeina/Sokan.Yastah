using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Users
{
    internal static class UsersLogMessages
    {
        public enum EventType
        {
            GrantedPermissionIdentitiesFetching     = BusinessLogEventType.Users + 0x0001,
            GrantedPermissionIdentitiesFetched      = BusinessLogEventType.Users + 0x0002,
            UserIsAdmin                             = BusinessLogEventType.Users + 0x0003,
            UserNotFound                            = BusinessLogEventType.Users + 0x0004,
            UserFound                               = BusinessLogEventType.Users + 0x0005,
            RoleMemberIdsFetching                   = BusinessLogEventType.Users + 0x0006,
            RoleMemberIdsFetched                    = BusinessLogEventType.Users + 0x0007,
            UserTracking                            = BusinessLogEventType.Users + 0x0008,
            UserTracked                             = BusinessLogEventType.Users + 0x0009,
            UserCreated                             = BusinessLogEventType.Users + 0x000A,
            DefaultPermissionIdsFetching            = BusinessLogEventType.Users + 0x000B,
            DefaultPermissionIdsFetched             = BusinessLogEventType.Users + 0x000C,
            DefaultRoleIdsFetching                  = BusinessLogEventType.Users + 0x000D,
            DefaultRoleIdsFetched                   = BusinessLogEventType.Users + 0x000E,
            UserInitializingNotificationPublishing  = BusinessLogEventType.Users + 0x000F,
            UserInitializingNotificationPublished   = BusinessLogEventType.Users + 0x0010,
            UserUpdating                            = BusinessLogEventType.Users + 0x0011,
            UserUpdateNoChangesGiven                = BusinessLogEventType.Users + 0x0012,
            UserUpdated                             = BusinessLogEventType.Users + 0x0013,
            PermissionIdsValidating                 = BusinessLogEventType.Users + 0x0014,
            PermissionIdsValidationFailed           = BusinessLogEventType.Users + 0x0015,
            PermissionIdsValidationSucceeded        = BusinessLogEventType.Users + 0x0016,
            RoleIdsValidating                       = BusinessLogEventType.Users + 0x0017,
            RoleIdsValidationFailed                 = BusinessLogEventType.Users + 0x0018,
            RoleIdsValidationSucceeded              = BusinessLogEventType.Users + 0x0019,
            UserPermissionMappingIdentitiesFetching = BusinessLogEventType.Users + 0x001A,
            UserPermissionMappingIdentitiesFetched  = BusinessLogEventType.Users + 0x001B,
            UserRoleMappingIdentitiesFetching       = BusinessLogEventType.Users + 0x001C,
            UserRoleMappingIdentitiesFetched        = BusinessLogEventType.Users + 0x001D,
            RoleMemberIdsCacheCleared               = BusinessLogEventType.Users + 0x001E,
            UserUpdatingNotificationPublishing      = BusinessLogEventType.Users + 0x001F,
            UserPermissionMappingsCreating          = BusinessLogEventType.Users + 0x0021,
            UserUpdatingNotificationPublished       = BusinessLogEventType.Users + 0x0020,
            UserPermissionMappingsCreated           = BusinessLogEventType.Users + 0x0022,
            UserPermissionMappingsDeleting          = BusinessLogEventType.Users + 0x0023,
            UserPermissionMappingsDeleted           = BusinessLogEventType.Users + 0x0024,
            UserRoleMappingsCreating                = BusinessLogEventType.Users + 0x0025,
            UserRoleMappingsCreated                 = BusinessLogEventType.Users + 0x0026,
            UserRoleMappingsDeleting                = BusinessLogEventType.Users + 0x0027,
            UserRoleMappingsDeleted                 = BusinessLogEventType.Users + 0x0028
        }


        public static void DefaultPermissionIdsFetched(
                ILogger logger,
                IEnumerable<int> permissionIds)
            => _defaultPermissionIdsFetched.Invoke(
                logger,
                permissionIds);
        private static readonly Action<ILogger, IEnumerable<int>> _defaultPermissionIdsFetched
            = LoggerMessage.Define<IEnumerable<int>>(
                    LogLevel.Debug,
                    EventType.DefaultPermissionIdsFetched.ToEventId(),
                    "DefaultPermissionMapping IDs fetched:\r\n\tPermissionIds: {PermissionIds}")
                .WithoutException();

        public static void DefaultPermissionIdsFetching(
                ILogger logger)
            => _defaultPermissionIdsFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultPermissionIdsFetching
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.DefaultPermissionIdsFetching.ToEventId(),
                    "Fetching DefaultPermissionMapping IDs")
                .WithoutException();

        public static void DefaultRoleIdsFetched(
                ILogger logger,
                IEnumerable<long> roleIds)
            => _defaultRoleIdsFetched.Invoke(
                logger,
                roleIds);
        private static readonly Action<ILogger, IEnumerable<long>> _defaultRoleIdsFetched
            = LoggerMessage.Define<IEnumerable<long>>(
                    LogLevel.Debug,
                    EventType.DefaultRoleIdsFetched.ToEventId(),
                    "DefaultRoleMapping IDs fetched:\r\n\tRoleIds: {RoleIds}")
                .WithoutException();

        public static void DefaultRoleIdsFetching(
                ILogger logger)
            => _defaultRoleIdsFetching.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultRoleIdsFetching
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.DefaultRoleIdsFetching.ToEventId(),
                    "Fetching DefaultRoleMapping IDs")
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
                    EventType.GrantedPermissionIdentitiesFetched.ToEventId(),
                    "Granted Permission IDs fetched:\r\n\tUserId: {UserId}")
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
                    EventType.GrantedPermissionIdentitiesFetching.ToEventId(),
                    "Fetching Granted Permission IDs:\r\n\tRoleId: {RoleId}")
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
                    EventType.PermissionIdsValidating.ToEventId(),
                    "Permission IDs validation succeded:\r\n\tPermissionIds: {PermissionIds}")
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
                    EventType.PermissionIdsValidationFailed.ToEventId(),
                    "Permission IDs validation failed:\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void PermissionIdsValidationSucceeded(
                ILogger logger)
            => _permissionIdsValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _permissionIdsValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.PermissionIdsValidationSucceeded.ToEventId(),
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
                    EventType.RoleIdsValidating.ToEventId(),
                    "Role IDs validation succeded:\r\n\tRoleIds: {RoleIds}")
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
                    EventType.RoleIdsValidationFailed.ToEventId(),
                    "Role IDs validation failed:\r\n\tValidationResult: {ValidationResult}")
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

        public static void RoleMemberIdsCacheCleared(
                ILogger logger,
                long roleId)
            => _roleMemberIdsCacheCleared.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleMemberIdsCacheCleared
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.RoleMemberIdsCacheCleared.ToEventId(),
                    "Role member IDs cache cleared:\r\n\tRoleId: {RoleId}")
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
                    EventType.RoleMemberIdsFetched.ToEventId(),
                    "Role member IDs fetched:\r\n\tMemberIds: {MemberIds}")
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
                    EventType.RoleMemberIdsFetching.ToEventId(),
                    "Fetching Role member IDs:\r\n\tRoleId: {RoleId}")
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
                    EventType.UserCreated.ToEventId(),
                    "User created: {UserId}")
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
                    EventType.UserFound.ToEventId(),
                    "User found: {UserId}")
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
                    EventType.UserInitializingNotificationPublished.ToEventId(),
                    $"{nameof(UserInitializingNotification)} published:\r\n\tUserId: {{UserId}}")
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
                    EventType.UserInitializingNotificationPublishing.ToEventId(),
                    $"Publishing {nameof(UserInitializingNotification)}:\r\n\tUserId: {{UserId}}")
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
                    EventType.UserIsAdmin.ToEventId(),
                    "User is administrator: skipping permissions lookup:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserNotFound(
                ILogger logger,
                ulong userId)
            => _userNotFound.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userNotFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    EventType.UserNotFound.ToEventId(),
                    "User not found: {UserId}")
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
                    EventType.UserPermissionMappingIdentitiesFetched.ToEventId(),
                    "UserPermissionMapping Identities fetched:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserPermissionMappingIdentitiesFetching(
                ILogger logger,
                ulong userId)
            => _userPermissionMappingIdentitiesFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userPermissionMappingIdentitiesFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.UserPermissionMappingIdentitiesFetching.ToEventId(),
                    "Fetching UserPermissionMapping Identities:\r\n\tUserId: {UserId}")
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
                    EventType.UserPermissionMappingsCreated.ToEventId(),
                    "UserPermissionMapping range created:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserPermissionMappingsCreating.ToEventId(),
                    "Creating UserPermissionMapping range:\r\n\tUserId: {UserId}\r\n\tPermissionIds: {PermissionIds}\r\n\tMappingType: {MappingType}")
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
                    EventType.UserPermissionMappingsDeleted.ToEventId(),
                    "UserPermissionMapping range deleted:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserPermissionMappingsDeleting.ToEventId(),
                    "Deleting UserPermissionMapping range:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserRoleMappingIdentitiesFetched.ToEventId(),
                    "UserRoleMapping Identities fetched:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void UserRoleMappingIdentitiesFetching(
                ILogger logger,
                ulong userId)
            => _userRoleMappingIdentitiesFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userRoleMappingIdentitiesFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.UserRoleMappingIdentitiesFetching.ToEventId(),
                    "Fetching UserRoleMapping Identities:\r\n\tUserId: {UserId}")
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
                    EventType.UserRoleMappingsCreated.ToEventId(),
                    "UserRoleMapping range created:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserRoleMappingsCreating.ToEventId(),
                    "Creating UserRoleMapping range:\r\n\tUserId: {UserId}\r\n\tRoleIds: {RoleIds}")
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
                    EventType.UserRoleMappingsDeleted.ToEventId(),
                    "UserRoleMapping range deleted:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserRoleMappingsDeleting.ToEventId(),
                    "Deleting UserRoleMapping range:\r\n\tUserId: {UserId}\r\n\tMappingIds: {MappingIds}")
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
                    EventType.UserUpdated.ToEventId(),
                    "User tracked:\r\n\tUserId: {UserId}")
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
                    EventType.UserTracking.ToEventId(),
                    "Tracking User:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}\r\n\tAvatarHash: {AvatarHash}")
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
                    EventType.UserUpdated.ToEventId(),
                    "User updated:\r\n\tUserId: {UserId}")
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
                    EventType.UserUpdateNoChangesGiven.ToEventId(),
                    "User update failed: No changes given:\r\n\tUserId: {UserId}")
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
                    EventType.UserUpdating.ToEventId(),
                    "Updating User:\r\n\tUserId: {UserId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
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
                    EventType.UserUpdatingNotificationPublished.ToEventId(),
                    $"{nameof(UserUpdatingNotification)} published:\r\n\tUserId: {{UserId}}")
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
                    EventType.UserUpdatingNotificationPublishing.ToEventId(),
                    $"Publishing {nameof(UserUpdatingNotification)}:\r\n\tUserId: {{UserId}}")
                .WithoutException();
    }
}
