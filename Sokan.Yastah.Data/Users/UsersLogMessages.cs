using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Users
{
    public static class UsersLogMessages
    {
        public enum EventType
        {
            UsersEnumeratingAny                         = DataLogEventType.Users + 0x0001,
            DefaultPermissionIdsEnumerating             = DataLogEventType.Users + 0x0002,
            DefaultRoleIdsEnumerating                   = DataLogEventType.Users + 0x0003,
            GrantedPermissionIdentitiesEnumerating      = DataLogEventType.Users + 0x0004,
            UserIdsEnumerating                          = DataLogEventType.Users + 0x0005,
            UserOverviewsEnumerating                    = DataLogEventType.Users + 0x0006,
            UserPermissionMappingIdentitiesEnumerating  = DataLogEventType.Users + 0x0007,
            UserRoleMappingIdentitiesEnumerating        = DataLogEventType.Users + 0x0008,
            UserPermissionMappingsCreating              = DataLogEventType.Users + 0x0009,
            UserPermissionMappingsCreated               = DataLogEventType.Users + 0x000A,
            UserPermissionMappingCreating               = DataLogEventType.Users + 0x000B,
            UserPermissionMappingCreated                = DataLogEventType.Users + 0x000C,
            UserRoleMappingsCreating                    = DataLogEventType.Users + 0x000D,
            UserRoleMappingsCreated                     = DataLogEventType.Users + 0x000E,
            UserRoleMappingCreating                     = DataLogEventType.Users + 0x000F,
            UserRoleMappingCreated                      = DataLogEventType.Users + 0x0010,
            UserMerging                                 = DataLogEventType.Users + 0x0011,
            UserMerged                                  = DataLogEventType.Users + 0x0012,
            UserCreating                                = DataLogEventType.Users + 0x0013,
            UserUpdating                                = DataLogEventType.Users + 0x0014,
            UserDetailReading                           = DataLogEventType.Users + 0x0015,
            UserDetailRead                              = DataLogEventType.Users + 0x0016,
            UserNotFound                                = DataLogEventType.Users + 0x0017,
            UserPermissionMappingsUpdating              = DataLogEventType.Users + 0x0018,
            UserPermissionMappingsUpdated               = DataLogEventType.Users + 0x0019,
            UserPermissionMappingUpdating               = DataLogEventType.Users + 0x001A,
            UserRoleMappingsUpdating                    = DataLogEventType.Users + 0x001B,
            UserRoleMappingsUpdated                     = DataLogEventType.Users + 0x001C,
            UserRoleMappingUpdating                     = DataLogEventType.Users + 0x001D
        }

        public static void DefaultPermissionIdsEnumerating(
                ILogger logger)
            => _defaultPermissionIdsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultPermissionIdsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.DefaultPermissionIdsEnumerating.ToEventId(),
                    $"Enumerating {nameof(DefaultPermissionMappingEntity)} IDs")
                .WithoutException();

        public static void DefaultRoleIdsEnumerating(
                ILogger logger)
            => _defaultRoleIdsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultRoleIdsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.DefaultRoleIdsEnumerating.ToEventId(),
                    $"Enumerating {nameof(DefaultRoleMappingEntity)} IDs")
                .WithoutException();

        public static void GrantedPermissionIdentitiesEnumerating(
                ILogger logger,
                ulong userId)
            => _grantedPermissionIdentitiesEnumerating.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _grantedPermissionIdentitiesEnumerating
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.GrantedPermissionIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(PermissionIdentityViewModel)} IDs: UserId: {{UserId}}")
                .WithoutException();

        public static void UserCreating(
                ILogger logger,
                ulong id,
                string username,
                string discriminator,
                string? avatarHash,
                DateTimeOffset firstSeen,
                DateTimeOffset lastSeen)
            => _userCreating.Invoke(
                logger,
                id,
                username,
                discriminator,
                avatarHash,
                firstSeen,
                lastSeen);
        private static Action<ILogger, ulong, string, string, string?, DateTimeOffset, DateTimeOffset> _userCreating
            = LoggerMessage.Define<ulong, string, string, string?, DateTimeOffset, DateTimeOffset>(
                    LogLevel.Information,
                    EventType.UserCreating.ToEventId(),
                    $"Creating {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tFirstSeen: {{FirstSeen}}\r\n\tLastSeen: {{LastSeen}}")
                .WithoutException();

        public static void UserDetailRead(
                ILogger logger,
                ulong userId)
            => _userDetailRead.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userDetailRead
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.UserDetailRead.ToEventId(),
                    $"{nameof(UserDetailViewModel)} read: {{UserId}}")
                .WithoutException();

        public static void UserDetailReading(
                ILogger logger,
                ulong userId)
            => _userDetailReading.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userDetailReading
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.UserDetailReading.ToEventId(),
                    $"Reading {nameof(UserDetailViewModel)}: {{UserId}}")
                .WithoutException();

        public static void UserIdsEnumerating(
                ILogger logger,
                Optional<long> roleId)
            => _userIdsEnumerating.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, Optional<long>> _userIdsEnumerating
            = LoggerMessage.Define<Optional<long>>(
                    LogLevel.Debug,
                    EventType.UserIdsEnumerating.ToEventId(),
                    $"Enumerating {nameof(UserEntity)} IDs:\r\n\tRoleId: {{RoleId}}")
                .WithoutException();

        public static void UserMerged(
                ILogger logger,
                ulong id)
            => _userMerged.Invoke(
                logger,
                id);
        private static Action<ILogger, ulong> _userMerged
            = LoggerMessage.Define<ulong>(
                    LogLevel.Information,
                    EventType.UserMerged.ToEventId(),
                    $"Merging {nameof(UserEntity)}: {{Id}}")
                .WithoutException();

        public static void UserMerging(
                ILogger logger,
                ulong id,
                string username,
                string discriminator,
                string? avatarHash,
                DateTimeOffset firstSeen,
                DateTimeOffset lastSeen)
            => _userMerging.Invoke(
                logger,
                id,
                username,
                discriminator,
                avatarHash,
                firstSeen,
                lastSeen);
        private static Action<ILogger, ulong, string, string, string?, DateTimeOffset, DateTimeOffset> _userMerging
            = LoggerMessage.Define<ulong, string, string, string?, DateTimeOffset, DateTimeOffset>(
                    LogLevel.Information,
                    EventType.UserMerging.ToEventId(),
                    $"Merging {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tFirstSeen: {{FirstSeen}}\r\n\tLastSeen: {{LastSeen}}")
                .WithoutException();

        public static void UserNotFound(
                ILogger logger,
                ulong userId)
            => _userNotFound.Invoke(
                logger,
                userId);
        private static Action<ILogger, ulong> _userNotFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    EventType.UserNotFound.ToEventId(),
                    $"{nameof(UserEntity)} not found: {{UserId}}")
                .WithoutException();

        public static void UserOverviewsEnumerating(
                ILogger logger)
            => _userOverviewsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _userOverviewsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.UserOverviewsEnumerating.ToEventId(),
                    $"Enumerating {nameof(UserOverviewViewModel)} IDs")
                .WithoutException();

        public static void UserPermissionMappingCreated(
                ILogger logger,
                long mappingId)
            => _userPermissionMappingCreated.Invoke(
                logger,
                mappingId);
        private static Action<ILogger, long> _userPermissionMappingCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.UserPermissionMappingCreated.ToEventId(),
                    $"{nameof(UserPermissionMappingEntity)} created: {{MappingId}}")
                .WithoutException();

        public static void UserPermissionMappingCreating(
                ILogger logger,
                ulong userId,
                int permissionId,
                PermissionMappingType type,
                long actionId)
            => _userPermissionMappingCreating.Invoke(
                logger,
                userId,
                permissionId,
                type,
                actionId);
        private static Action<ILogger, ulong, int, PermissionMappingType, long> _userPermissionMappingCreating
            = LoggerMessage.Define<ulong, int, PermissionMappingType, long>(
                    LogLevel.Information,
                    EventType.UserPermissionMappingCreating.ToEventId(),
                    $"Creating {nameof(UserPermissionMappingEntity)}:\r\n\tUserId: {{UserId}}\r\n\tPermissionId: {{PermissionId}}\r\n\tType: {{Type}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void UserPermissionMappingIdentitiesEnumerating(
                ILogger logger,
                ulong userId,
                Optional<bool> isDeleted)
            => _userPermissionMappingIdentitiesEnumerating.Invoke(
                logger,
                userId,
                isDeleted);
        private static readonly Action<ILogger, ulong, Optional<bool>> _userPermissionMappingIdentitiesEnumerating
            = LoggerMessage.Define<ulong, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.UserPermissionMappingIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(UserPermissionMappingIdentityViewModel)}:\r\n\tUserId: {{UserId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void UserPermissionMappingsCreated(
                ILogger logger)
            => _userPermissionMappingsCreated.Invoke(
                logger);
        private static Action<ILogger> _userPermissionMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.UserPermissionMappingsCreated.ToEventId(),
                    $"{nameof(UserPermissionMappingEntity)} range created")
                .WithoutException();

        public static void UserPermissionMappingsCreating(
                ILogger logger,
                ulong userId,
                IEnumerable<int> permissionIds,
                PermissionMappingType type,
                long actionId)
            => _userPermissionMappingsCreating.Invoke(
                logger,
                userId,
                permissionIds,
                type,
                actionId);
        private static Action<ILogger, ulong, IEnumerable<int>, PermissionMappingType, long> _userPermissionMappingsCreating
            = LoggerMessage.Define<ulong, IEnumerable<int>, PermissionMappingType, long>(
                    LogLevel.Information,
                    EventType.UserPermissionMappingsCreating.ToEventId(),
                    $"Creating {nameof(UserPermissionMappingEntity)} Range:\r\n\tUserId: {{UserId}}\r\n\tPermissionIds: {{PermissionIds}}\r\n\tType: {{Type}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void UserPermissionMappingsUpdated(
                ILogger logger)
            => _userPermissionMappingsUpdated.Invoke(
                logger);
        private static Action<ILogger> _userPermissionMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.UserPermissionMappingsUpdated.ToEventId(),
                    $"{nameof(UserPermissionMappingEntity)} range created")
                .WithoutException();

        public static void UserPermissionMappingsUpdating(
                ILogger logger,
                IEnumerable<long> mappingIds,
                long deletionId)
            => _userPermissionMappingsUpdating.Invoke(
                logger,
                mappingIds,
                deletionId);
        private static Action<ILogger, IEnumerable<long>, long> _userPermissionMappingsUpdating
            = LoggerMessage.Define<IEnumerable<long>, long>(
                    LogLevel.Information,
                    EventType.UserPermissionMappingsUpdating.ToEventId(),
                    $"Updating {nameof(UserPermissionMappingEntity)} Range:\r\n\tMappingIds: {{MappingIds}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UserPermissionMappingUpdating(
                ILogger logger,
                long mappingId,
                long deletionId)
            => _userPermissionMappingUpdating.Invoke(
                logger,
                mappingId,
                deletionId);
        private static Action<ILogger, long, long> _userPermissionMappingUpdating
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.UserPermissionMappingUpdating.ToEventId(),
                    $"Updating {nameof(UserPermissionMappingEntity)}:\r\n\tMappingId: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UserRoleMappingCreated(
                ILogger logger,
                long mappingId)
            => _userRoleMappingCreated.Invoke(
                logger,
                mappingId);
        private static Action<ILogger, long> _userRoleMappingCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.UserRoleMappingCreated.ToEventId(),
                    $"{nameof(UserRoleMappingEntity)} created: {{MappingId}}")
                .WithoutException();

        public static void UserRoleMappingCreating(
                ILogger logger,
                ulong userId,
                long roleId,
                long actionId)
            => _userRoleMappingCreating.Invoke(
                logger,
                userId,
                roleId,
                actionId);
        private static Action<ILogger, ulong, long, long> _userRoleMappingCreating
            = LoggerMessage.Define<ulong, long, long>(
                    LogLevel.Information,
                    EventType.UserRoleMappingCreating.ToEventId(),
                    $"Creating {nameof(UserRoleMappingEntity)}:\r\n\tUserId: {{UserId}}\r\n\tRoleId: {{RoleId}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void UserRoleMappingIdentitiesEnumerating(
                ILogger logger,
                ulong userId,
                Optional<bool> isDeleted)
            => _userRoleMappingIdentitiesEnumerating.Invoke(
                logger,
                userId,
                isDeleted);
        private static readonly Action<ILogger, ulong, Optional<bool>> _userRoleMappingIdentitiesEnumerating
            = LoggerMessage.Define<ulong, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.UserRoleMappingIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(UserRoleMappingIdentityViewModel)}:\r\n\tUserId: {{UserId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void UserRoleMappingsCreated(
                ILogger logger)
            => _userRoleMappingsCreated.Invoke(
                logger);
        private static Action<ILogger> _userRoleMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.UserRoleMappingsCreated.ToEventId(),
                    $"{nameof(UserRoleMappingEntity)} range created")
                .WithoutException();

        public static void UserRoleMappingsCreating(
                ILogger logger,
                ulong userId,
                IEnumerable<long> roleIds,
                long actionId)
            => _userRoleMappingsCreating.Invoke(
                logger,
                userId,
                roleIds,
                actionId);
        private static Action<ILogger, ulong, IEnumerable<long>, long> _userRoleMappingsCreating
            = LoggerMessage.Define<ulong, IEnumerable<long>, long>(
                    LogLevel.Information,
                    EventType.UserRoleMappingsCreating.ToEventId(),
                    $"Creating {nameof(UserRoleMappingEntity)} Range:\r\n\tUserId: {{UserID}}\r\n\tRoleIds: {{RoleIds}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void UserRoleMappingsUpdated(
                ILogger logger)
            => _userRoleMappingsUpdated.Invoke(
                logger);
        private static Action<ILogger> _userRoleMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.UserRoleMappingsUpdated.ToEventId(),
                    $"{nameof(UserRoleMappingEntity)} range created")
                .WithoutException();

        public static void UserRoleMappingsUpdating(
                ILogger logger,
                IEnumerable<long> mappingIds,
                long deletionId)
            => _userRoleMappingsUpdating.Invoke(
                logger,
                mappingIds,
                deletionId);
        private static Action<ILogger, IEnumerable<long>, long> _userRoleMappingsUpdating
            = LoggerMessage.Define<IEnumerable<long>, long>(
                    LogLevel.Information,
                    EventType.UserRoleMappingsUpdating.ToEventId(),
                    $"Updating {nameof(UserRoleMappingEntity)} Range:\r\n\tMappingIds: {{MappingIds}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UserRoleMappingUpdating(
                ILogger logger,
                long mappingId,
                long deletionId)
            => _userRoleMappingUpdating.Invoke(
                logger,
                mappingId,
                deletionId);
        private static Action<ILogger, long, long> _userRoleMappingUpdating
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.UserRoleMappingUpdating.ToEventId(),
                    $"Updating {nameof(UserRoleMappingEntity)}:\r\n\tMappingId: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UsersEnumeratingAny(
                ILogger logger,
                Optional<ulong> userId)
            => _usersEnumeratingAny.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, Optional<ulong>> _usersEnumeratingAny
            = LoggerMessage.Define<Optional<ulong>>(
                    LogLevel.Debug,
                    EventType.UsersEnumeratingAny.ToEventId(),
                    $"Enumerating for any {nameof(UserEntity)}: UserId: {{UserId}}")
                .WithoutException();

        public static void UserUpdating(
                ILogger logger,
                ulong id,
                string username,
                string discriminator,
                string? avatarHash,
                DateTimeOffset lastSeen)
            => _userUpdating.Invoke(
                logger,
                id,
                username,
                discriminator,
                avatarHash,
                lastSeen);
        private static Action<ILogger, ulong, string, string, string?, DateTimeOffset> _userUpdating
            = LoggerMessage.Define<ulong, string, string, string?, DateTimeOffset>(
                    LogLevel.Information,
                    EventType.UserUpdating.ToEventId(),
                    $"Updating {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tLastSeen: {{LastSeen}}")
                .WithoutException();
    }
}
