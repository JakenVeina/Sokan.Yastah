using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Administration;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public interface IUsersService
    {
        Task<OperationResult<IReadOnlyCollection<PermissionIdentityViewModel>>> GetGrantedPermissionsAsync(
            ulong userId,
            CancellationToken cancellationToken);

        ValueTask<IReadOnlyCollection<ulong>> GetRoleMemberIdsAsync(
            long roleId,
            CancellationToken cancellationToken);

        Task TrackUserAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class UsersService
        : IUsersService
    {
        public UsersService(
            IAdministrationActionsRepository administrationActionsRepository,
            IOptions<AuthorizationConfiguration> authorizationConfigurationOptions,
            ILogger<UsersService> logger,
            IMemoryCache memoryCache,
            IMessenger messenger,
            IPermissionsService permissionsService,
            IRolesService rolesService,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory,
            IUsersRepository usersRepository)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _authorizationConfigurationOptions = authorizationConfigurationOptions;
            _logger = logger;
            _memoryCache = memoryCache;
            _messenger = messenger;
            _permissionsService = permissionsService;
            _rolesService = rolesService;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
            _usersRepository = usersRepository;
        }

        public async Task<OperationResult<IReadOnlyCollection<PermissionIdentityViewModel>>> GetGrantedPermissionsAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.GrantedPermissionIdentitiesFetching(_logger, userId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            if (_authorizationConfigurationOptions.Value.AdminUserIds.Contains(userId))
            {
                UsersLogMessages.UserIsAdmin(_logger, userId);
                return (await _permissionsService.GetIdentitiesAsync(cancellationToken))
                    .ToSuccess();
            }

            var userExists = await _usersRepository.AnyAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            if (!userExists)
            {
                UsersLogMessages.UserNotFound(_logger, userId);
                return new DataNotFoundError($"User ID {userId}");
            }

            UsersLogMessages.UserFound(_logger, userId);
            var identities = await _usersRepository.AsyncEnumerateGrantedPermissionIdentities(userId)
                .ToArrayAsync(cancellationToken);
            UsersLogMessages.GrantedPermissionIdentitiesFetched(_logger, userId);
            
            return identities
                .ToSuccess<IReadOnlyCollection<PermissionIdentityViewModel>>();
        }

        public ValueTask<IReadOnlyCollection<ulong>> GetRoleMemberIdsAsync(
                long roleId,
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync<IReadOnlyCollection<ulong>>(MakeRoleMemberIdsCacheKey(roleId), async entry =>
            {
                UsersLogMessages.RoleMemberIdsFetching(_logger, roleId);
                entry.Priority = CacheItemPriority.High;

                var memberIds = await _usersRepository.AsyncEnumerateIds(
                        roleId: roleId)
                    .ToArrayAsync(cancellationToken);
                UsersLogMessages.RoleMemberIdsFetched(_logger, memberIds);

                return memberIds;
            });

        public async Task TrackUserAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserTracking(_logger, userId, username, discriminator, avatarHash);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);
            
            var now = _systemClock.UtcNow;

            var mergeResult = await _usersRepository.MergeAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                firstSeen: now,
                lastSeen: now,
                cancellationToken);

            if (mergeResult.RowsInserted > 0)
            {
                UsersLogMessages.UserCreated(_logger, userId);

                var actionId = await _administrationActionsRepository.CreateAsync(
                    (int)UserManagementAdministrationActionType.UserCreated,
                    now,
                    userId,
                    cancellationToken);
                AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

                UsersLogMessages.DefaultPermissionIdsFetching(_logger);
                var defaultPermissionIds = await _usersRepository
                        .AsyncEnumerateDefaultPermissionIds()
                    .ToArrayAsync(cancellationToken);
                UsersLogMessages.DefaultPermissionIdsFetched(_logger, defaultPermissionIds);

                if (defaultPermissionIds.Any())
                {
                    UsersLogMessages.UserPermissionMappingsCreating(_logger, userId, defaultPermissionIds, PermissionMappingType.Granted);
                    var mappingIds = await _usersRepository.CreatePermissionMappingsAsync(
                        userId,
                        defaultPermissionIds,
                        PermissionMappingType.Granted,
                        actionId,
                        cancellationToken);
                    UsersLogMessages.UserPermissionMappingsCreated(_logger, userId, mappingIds);
                }

                UsersLogMessages.DefaultRoleIdsFetching(_logger);
                var defaultRoleIds = await _usersRepository
                        .AsyncEnumerateDefaultRoleIds()
                    .ToArrayAsync(cancellationToken);
                UsersLogMessages.DefaultRoleIdsFetched(_logger, defaultRoleIds);

                if (defaultRoleIds.Any())
                {
                    UsersLogMessages.UserRoleMappingsCreating(_logger, userId, defaultRoleIds);
                    var mappingIds = await _usersRepository.CreateRoleMappingsAsync(
                        userId,
                        defaultRoleIds,
                        actionId,
                        cancellationToken);
                    UsersLogMessages.UserRoleMappingsCreated(_logger, userId, mappingIds);
                }

                UsersLogMessages.UserInitializingNotificationPublishing(_logger, userId);
                await _messenger.PublishNotificationAsync(
                    new UserInitializingNotification(
                        userId,
                        actionId),
                    cancellationToken);
                UsersLogMessages.UserInitializingNotificationPublished(_logger, userId);
            }
            else
                UsersLogMessages.UserUpdated(_logger, userId);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            UsersLogMessages.UserTracked(_logger, userId);
        }

        public async Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserUpdating(_logger, userId, updateModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var permissionIds = Enumerable.Union(
                    updateModel.GrantedPermissionIds,
                    updateModel.DeniedPermissionIds)
                .ToArray();
            UsersLogMessages.PermissionIdsValidating(_logger, permissionIds);
            var permissionIdsValidationResult = await _permissionsService.ValidateIdsAsync(
                permissionIds,
                cancellationToken);
            if (permissionIdsValidationResult.IsFailure)
            {
                UsersLogMessages.PermissionIdsValidationFailed(_logger, permissionIdsValidationResult);
                return permissionIdsValidationResult;
            }
            UsersLogMessages.PermissionIdsValidationSucceeded(_logger);

            UsersLogMessages.RoleIdsValidating(_logger, updateModel.AssignedRoleIds);
            var assignedRoleIdsValidationResult = await _rolesService.ValidateIdsAsync(updateModel.AssignedRoleIds, cancellationToken);
            if (assignedRoleIdsValidationResult.IsFailure)
            {
                UsersLogMessages.RoleIdsValidationFailed(_logger, assignedRoleIdsValidationResult);
                return assignedRoleIdsValidationResult;
            }
            UsersLogMessages.RoleIdsValidationSucceeded(_logger);

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)UserManagementAdministrationActionType.UserModified,
                now,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var anyChanges = false;

            UsersLogMessages.UserPermissionMappingIdentitiesFetching(_logger, userId);
            var permissionMappings = await _usersRepository.AsyncEnumeratePermissionMappingIdentities(
                    userId: userId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);
            UsersLogMessages.UserPermissionMappingIdentitiesFetched(_logger, userId);

            anyChanges |= await HandleRemovedPermissionMappings(
                userId,
                permissionMappings,
                updateModel.GrantedPermissionIds,
                updateModel.DeniedPermissionIds,
                actionId,
                cancellationToken);

            anyChanges |= await HandleAddedGrantedPermissions(
                permissionMappings,
                updateModel.GrantedPermissionIds,
                userId,
                actionId,
                cancellationToken);

            anyChanges |= await HandleAddedDeniedPermissions(
                permissionMappings,
                updateModel.DeniedPermissionIds,
                userId,
                actionId,
                cancellationToken);

            UsersLogMessages.UserRoleMappingIdentitiesFetching(_logger, userId);
            var roleMappings = await _usersRepository.AsyncEnumerateRoleMappingIdentities(
                    userId: userId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);
            UsersLogMessages.UserRoleMappingIdentitiesFetched(_logger, userId);

            anyChanges |= await HandleRemovedRoleMappings(
                userId,
                roleMappings,
                updateModel.AssignedRoleIds,
                actionId,
                cancellationToken);

            anyChanges |= await HandleAddedRoles(
                roleMappings,
                updateModel.AssignedRoleIds,
                userId,
                actionId,
                cancellationToken);

            if (!anyChanges)
            {
                UsersLogMessages.UserUpdateNoChangesGiven(_logger, userId);
                return new NoChangesGivenError($"User ID {userId}");
            }

            UsersLogMessages.UserUpdatingNotificationPublishing(_logger, userId);
            await _messenger.PublishNotificationAsync(
                new UserUpdatingNotification(
                    userId,
                    actionId),
                cancellationToken);
            UsersLogMessages.UserUpdatingNotificationPublished(_logger, userId);
            
            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            UsersLogMessages.UserUpdated(_logger, userId);
            return OperationResult.Success;
        }

        private async Task<bool> HandleAddedDeniedPermissions(
            IEnumerable<UserPermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> deniedPermissionIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedDeniedPermissionIds = deniedPermissionIds
                .Where(id => !permissionMappings.Any(x => x.IsDenied && (x.PermissionId == id)))
                .ToArray();

            if (!addedDeniedPermissionIds.Any())
                return false;

            UsersLogMessages.UserPermissionMappingsCreating(_logger, userId, addedDeniedPermissionIds, PermissionMappingType.Denied);
            var mappingIds = await _usersRepository.CreatePermissionMappingsAsync(
                userId,
                addedDeniedPermissionIds,
                PermissionMappingType.Denied,
                actionId,
                cancellationToken);
            UsersLogMessages.UserPermissionMappingsCreated(_logger, userId, mappingIds);

            return true;
        }

        private async Task<bool> HandleAddedGrantedPermissions(
            IEnumerable<UserPermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedGrantedPermissionIds = grantedPermissionIds
                .Where(id => !permissionMappings.Any(x => !x.IsDenied && (x.PermissionId == id)))
                .ToArray();

            if (!addedGrantedPermissionIds.Any())
                return false;

            UsersLogMessages.UserPermissionMappingsCreating(_logger, userId, addedGrantedPermissionIds, PermissionMappingType.Granted);
            var mappingIds = await _usersRepository.CreatePermissionMappingsAsync(
                userId,
                addedGrantedPermissionIds,
                PermissionMappingType.Granted,
                actionId,
                cancellationToken);
            UsersLogMessages.UserPermissionMappingsCreated(_logger, userId, mappingIds);

            return true;
        }

        private async Task<bool> HandleAddedRoles(
            IEnumerable<UserRoleMappingIdentityViewModel> roleMappings,
            IEnumerable<long> assignedRoleIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedRoleIds = assignedRoleIds
                .Where(id => !roleMappings.Any(x => x.RoleId == id))
                .ToArray();

            if (!addedRoleIds.Any())
                return false;

            UsersLogMessages.UserRoleMappingsCreating(_logger, userId, addedRoleIds);
            var mappingIds = await _usersRepository.CreateRoleMappingsAsync(
                userId,
                addedRoleIds,
                actionId,
                cancellationToken);
            UsersLogMessages.UserRoleMappingsCreated(_logger, userId, mappingIds);

            foreach (var roleId in addedRoleIds)
            {
                _memoryCache.Remove(MakeRoleMemberIdsCacheKey(roleId));
                UsersLogMessages.RoleMemberIdsCacheCleared(_logger, roleId);
            }

            return true;
        }

        private async Task<bool> HandleRemovedPermissionMappings(
            ulong userId,
            IEnumerable<UserPermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            IEnumerable<int> deniedPermissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedPermissionMappingIds = permissionMappings
                .Where(x => (!x.IsDenied && !grantedPermissionIds.Contains(x.PermissionId))
                    || (x.IsDenied && !deniedPermissionIds.Contains(x.PermissionId)))
                .Select(x => x.Id);

            if (!removedPermissionMappingIds.Any())
                return false;

            UsersLogMessages.UserPermissionMappingsDeleting(_logger, userId, removedPermissionMappingIds);
            await _usersRepository.UpdatePermissionMappingsAsync(
                removedPermissionMappingIds,
                actionId,
                cancellationToken);
            UsersLogMessages.UserPermissionMappingsDeleted(_logger, userId, removedPermissionMappingIds);

            return true;
        }

        private async Task<bool> HandleRemovedRoleMappings(
            ulong userId,
            IEnumerable<UserRoleMappingIdentityViewModel> roleMappings,
            IEnumerable<long> assignedRoleIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedRoleMappings = roleMappings
                .Where(x => !assignedRoleIds.Contains(x.RoleId))
                .ToArray();

            if (!removedRoleMappings.Any())
                return false;

            var mappingIds = removedRoleMappings
                .Select(x => x.Id);

            UsersLogMessages.UserRoleMappingsDeleting(_logger, userId, mappingIds);
            await _usersRepository.UpdateRoleMappingsAsync(
                mappingIds,
                actionId,
                cancellationToken);
            UsersLogMessages.UserRoleMappingsDeleted(_logger, userId, mappingIds);

            foreach (var mapping in removedRoleMappings)
            {
                _memoryCache.Remove(MakeRoleMemberIdsCacheKey(mapping.RoleId));
                UsersLogMessages.RoleMemberIdsCacheCleared(_logger, mapping.RoleId);
            }

            return true;
        }

        internal static string MakeRoleMemberIdsCacheKey(long roleId)
            => $"{nameof(UsersService)}.RoleMemberIds.{roleId}";

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly IOptions<AuthorizationConfiguration> _authorizationConfigurationOptions;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMessenger _messenger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRolesService _rolesService;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IUsersRepository _usersRepository;
    }
}
