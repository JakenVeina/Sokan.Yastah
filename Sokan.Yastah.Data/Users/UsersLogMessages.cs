using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Users
{
    public static class UsersLogMessages
    {
        public static void UserNotFound(
                ILogger logger,
                ulong userId)
            => _userVersionNotFound.Invoke(
                logger,
                userId);
        private static Action<ILogger, ulong> _userVersionNotFound
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(UserNotFound)),
                    $"{nameof(UserEntity)} not found: {{UserId}}")
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
                    new EventId(3001, nameof(UserPermissionMappingsCreating)),
                    $"Creating {nameof(UserPermissionMappingEntity)} Range:\r\n\tUserId: {{UserId}}\r\n\tPermissionIds: {{PermissionIds}}\r\n\tType: {{Type}}\r\n\tActionId: {{ActionId}}")
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
                    new EventId(3002, nameof(UserPermissionMappingCreating)),
                    $"Creating {nameof(UserPermissionMappingEntity)}:\r\n\tUserId: {{UserId}}\r\n\tPermissionId: {{PermissionId}}\r\n\tType: {{Type}}\r\n\tActionId: {{ActionId}}")
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
                    new EventId(3003, nameof(UserPermissionMappingCreated)),
                    $"{nameof(UserPermissionMappingEntity)} created: {{MappingId}}")
                .WithoutException();

        public static void UserPermissionMappingsCreated(
                ILogger logger)
            => _userPermissionMappingsCreated.Invoke(
                logger);
        private static Action<ILogger> _userPermissionMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3004, nameof(UserPermissionMappingsCreated)),
                    $"{nameof(UserPermissionMappingEntity)} range created")
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
                    new EventId(3005, nameof(UserRoleMappingsCreating)),
                    $"Creating {nameof(UserRoleMappingEntity)} Range:\r\n\tUserId: {{UserID}}\r\n\tRoleIds: {{RoleIds}}\r\n\tActionId: {{ActionId}}")
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
                    new EventId(3006, nameof(UserRoleMappingCreating)),
                    $"Creating {nameof(UserRoleMappingEntity)}:\r\n\tUserId: {{UserId}}\r\n\tRoleId: {{RoleId}}\r\n\tActionId: {{ActionId}}")
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
                    new EventId(3007, nameof(UserRoleMappingCreated)),
                    $"{nameof(UserRoleMappingEntity)} created: {{MappingId}}")
                .WithoutException();

        public static void UserRoleMappingsCreated(
                ILogger logger)
            => _userRoleMappingsCreated.Invoke(
                logger);
        private static Action<ILogger> _userRoleMappingsCreated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3008, nameof(UserRoleMappingsCreated)),
                    $"{nameof(UserRoleMappingEntity)} range created")
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
                    new EventId(3009, nameof(UserMerging)),
                    $"Merging {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tFirstSeen: {{FirstSeen}}\r\n\tLastSeen: {{LastSeen}}")
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
                    new EventId(3010, nameof(UserMerged)),
                    $"Merging {nameof(UserEntity)}: {{Id}}")
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
                    new EventId(3011, nameof(UserCreating)),
                    $"Creating {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tFirstSeen: {{FirstSeen}}\r\n\tLastSeen: {{LastSeen}}")
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
                    new EventId(3012, nameof(UserUpdating)),
                    $"Updating {nameof(UserEntity)}:\r\n\tId: {{Id}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tLastSeen: {{LastSeen}}")
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
                    new EventId(3013, nameof(UserPermissionMappingsUpdating)),
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
                    new EventId(3014, nameof(UserPermissionMappingUpdating)),
                    $"Updating {nameof(UserPermissionMappingEntity)}:\r\n\tMappingId: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UserPermissionMappingsUpdated(
                ILogger logger)
            => _userPermissionMappingsUpdated.Invoke(
                logger);
        private static Action<ILogger> _userPermissionMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3016, nameof(UserPermissionMappingsUpdated)),
                    $"{nameof(UserPermissionMappingEntity)} range created")
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
                    new EventId(3013, nameof(UserRoleMappingsUpdating)),
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
                    new EventId(3014, nameof(UserRoleMappingUpdating)),
                    $"Updating {nameof(UserRoleMappingEntity)}:\r\n\tMappingId: {{MappingId}}\r\n\tDeletionId: {{DeletionId}}")
                .WithoutException();

        public static void UserRoleMappingsUpdated(
                ILogger logger)
            => _userRoleMappingsUpdated.Invoke(
                logger);
        private static Action<ILogger> _userRoleMappingsUpdated
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3016, nameof(UserRoleMappingsUpdated)),
                    $"{nameof(UserRoleMappingEntity)} range created")
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
                    new EventId(4001, nameof(UsersEnumeratingAny)),
                    $"Enumerating for any {nameof(UserEntity)}: UserId: {{UserId}}")
                .WithoutException();

        public static void DefaultPermissionIdsEnumerating(
                ILogger logger)
            => _defaultPermissionIdsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultPermissionIdsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(DefaultPermissionIdsEnumerating)),
                    $"Enumerating {nameof(DefaultPermissionMappingEntity)} IDs")
                .WithoutException();

        public static void DefaultRoleIdsEnumerating(
                ILogger logger)
            => _defaultRoleIdsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _defaultRoleIdsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4003, nameof(DefaultRoleIdsEnumerating)),
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
                    new EventId(4004, nameof(GrantedPermissionIdentitiesEnumerating)),
                    $"Enumerating {nameof(PermissionIdentityViewModel)} IDs: UserId: {{UserId}}")
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
                    new EventId(4005, nameof(UserIdsEnumerating)),
                    $"Enumerating {nameof(UserEntity)} IDs:\r\n\tRoleId: {{RoleId}}")
                .WithoutException();

        public static void UserOverviewsEnumerating(
                ILogger logger)
            => _userOverviewsEnumerating.Invoke(
                logger);
        private static readonly Action<ILogger> _userOverviewsEnumerating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4006, nameof(UserOverviewsEnumerating)),
                    $"Enumerating {nameof(UserOverviewViewModel)} IDs")
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
                    new EventId(4007, nameof(UserPermissionMappingIdentitiesEnumerating)),
                    $"Enumerating {nameof(UserPermissionMappingIdentityViewModel)}:\r\n\tUserId: {{UserId}}\r\n\tIsDeleted: {{IsDeleted}}")
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
                    new EventId(4008, nameof(UserRoleMappingIdentitiesEnumerating)),
                    $"Enumerating {nameof(UserRoleMappingIdentityViewModel)}:\r\n\tUserId: {{UserId}}\r\n\tIsDeleted: {{IsDeleted}}")
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
                    new EventId(4009, nameof(UserDetailReading)),
                    $"Reading {nameof(UserDetailViewModel)}: {{UserId}}")
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
                    new EventId(4010, nameof(UserDetailRead)),
                    $"{nameof(UserDetailViewModel)} read: {{UserId}}")
                .WithoutException();
    }
}
