using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Administration;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Business.Roles
{
    public interface IRolesService
    {
        Task<OperationResult<long>> CreateAsync(
            RoleCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long roleId,
            ulong performedById,
            CancellationToken cancellationToken);

        ValueTask<IReadOnlyCollection<RoleIdentityViewModel>> GetCurrentIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);

        ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<long> roleIds,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class RolesService
        : IRolesService
    {
        public RolesService(
            IAdministrationActionsRepository administrationActionsRepository,
            ILogger<RolesService> logger,
            IMemoryCache memoryCache,
            IMessenger messenger,
            IPermissionsService permissionsService,
            IRolesRepository rolesRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _messenger = messenger;
            _permissionsService = permissionsService;
            _rolesRepository = rolesRepository;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<OperationResult<long>> CreateAsync(
            RoleCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            RolesLogMessages.RoleCreating(_logger, creationModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var nameValidationResult = await ValidateNameAsync(creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                RolesLogMessages.RoleNameValidationFailed(_logger, creationModel.Name, nameValidationResult);
                return nameValidationResult.Error;
            }
            RolesLogMessages.RoleNameValidationSucceeded(_logger, creationModel.Name);

            var grantedPermissionIdsValidationResult = await _permissionsService.ValidateIdsAsync(creationModel.GrantedPermissionIds, cancellationToken);
            if (grantedPermissionIdsValidationResult.IsFailure)
            {
                RolesLogMessages.PermissionIdsValidationFailed(_logger, creationModel.GrantedPermissionIds, grantedPermissionIdsValidationResult);
                return grantedPermissionIdsValidationResult.Error;
            }
            RolesLogMessages.PermissionIdsValidationSucceeded(_logger, creationModel.GrantedPermissionIds);

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var roleId = await _rolesRepository.CreateAsync(
                creationModel.Name,
                actionId,
                cancellationToken);
            RolesLogMessages.RoleCreated(_logger, roleId);

            var mappingIds = await _rolesRepository.CreatePermissionMappingsAsync(
                roleId,
                creationModel.GrantedPermissionIds,
                actionId,
                cancellationToken);
            RolesLogMessages.RolePermissionMappingsCreated(_logger, roleId, mappingIds);

            _memoryCache.Remove(_getCurrentIdentitiesCacheKey);
            RolesLogMessages.RoleIdentitiesCacheCleared(_logger);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            return roleId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
            long roleId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            RolesLogMessages.RoleDeleting(_logger, roleId, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var deleteResult = await _rolesRepository.UpdateAsync(
                roleId: roleId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (deleteResult.IsSuccess)
            {
                RolesLogMessages.RoleDeleted(_logger, roleId, deleteResult.Value);

                _memoryCache.Remove(_getCurrentIdentitiesCacheKey);
                RolesLogMessages.RoleIdentitiesCacheCleared(_logger);

                transactionScope.Complete();
                TransactionsLogMessages.TransactionScopeCommitted(_logger);
            }
            else
                RolesLogMessages.RoleDeleteFailed(_logger, roleId, deleteResult);

            return deleteResult;
        }

        public ValueTask<IReadOnlyCollection<RoleIdentityViewModel>> GetCurrentIdentitiesAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync<IReadOnlyCollection<RoleIdentityViewModel>>(_getCurrentIdentitiesCacheKey, async entry =>
            {
                using var logScope = _logger.BeginMemberScope(nameof(GetCurrentIdentitiesAsync));
                RolesLogMessages.RoleIdentitiesFetchingCurrent(_logger);

                entry.Priority = CacheItemPriority.High;

                var result = await _rolesRepository.AsyncEnumerateIdentities(
                        isDeleted: false)
                    .ToArrayAsync(cancellationToken);
                RolesLogMessages.RoleIdentitiesFetchedCurrent(_logger);

                return result;
            });

        public async Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            RolesLogMessages.RoleUpdating(_logger, roleId, updateModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var nameValidationResult = await ValidateNameAsync(updateModel.Name, roleId, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                RolesLogMessages.RoleNameValidationFailed(_logger, updateModel.Name, nameValidationResult);
                return nameValidationResult;
            }
            RolesLogMessages.RoleNameValidationSucceeded(_logger, updateModel.Name);

            var grantedPermissionIdsValidationResult = await _permissionsService.ValidateIdsAsync(updateModel.GrantedPermissionIds, cancellationToken);
            if (grantedPermissionIdsValidationResult.IsFailure)
            {
                RolesLogMessages.PermissionIdsValidationFailed(_logger, updateModel.GrantedPermissionIds, grantedPermissionIdsValidationResult);
                return grantedPermissionIdsValidationResult;
            }
            RolesLogMessages.PermissionIdsValidationSucceeded(_logger, updateModel.GrantedPermissionIds);

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleModified,
                now,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var updateResult = await _rolesRepository.UpdateAsync(
                roleId: roleId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);
            
            if (updateResult.IsFailure && !(updateResult.Error is NoChangesGivenError))
            {
                RolesLogMessages.RoleUpdateFailed(_logger, roleId, updateResult);
                return updateResult;
            }
            RolesLogMessages.RoleUpdated(_logger, roleId, updateResult);

            var anyChanges = updateResult.IsSuccess;

            RolesLogMessages.RolePermissionMappingIdentitiesFetching(_logger, roleId);
            var permissionMappings = await _rolesRepository.AsyncEnumeratePermissionMappingIdentities(
                    roleId: roleId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);
            RolesLogMessages.RolePermissionMappingIdentitiesFetched(_logger, roleId);

            anyChanges |= await HandleRemovedPermissionMappings(
                roleId,
                permissionMappings,
                updateModel.GrantedPermissionIds,
                actionId,
                cancellationToken);

            anyChanges |= await HandleAddedPermissions(
                permissionMappings,
                updateModel.GrantedPermissionIds,
                roleId,
                actionId,
                cancellationToken);

            if (!anyChanges)
            {
                RolesLogMessages.RoleUpdateNoChangesGiven(_logger, roleId);
                return new NoChangesGivenError($"Role ID {roleId}");
            }

            RolesLogMessages.RoleUpdatingNotificationPublishing(_logger, roleId);
            await _messenger.PublishNotificationAsync(
                new RoleUpdatingNotification(
                    roleId,
                    actionId),
                cancellationToken);
            RolesLogMessages.RoleUpdatingNotificationPublished(_logger, roleId);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            _memoryCache.Remove(_getCurrentIdentitiesCacheKey);
            RolesLogMessages.RoleIdentitiesCacheCleared(_logger);

            return OperationResult.Success;
        }

        public async ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<long> roleIds,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            RolesLogMessages.RoleIdsValidating(_logger, roleIds);

            if (!roleIds.Any())
            {
                RolesLogMessages.RoleIdsValidationSucceeded(_logger);
                return OperationResult.Success;
            }

            var invalidRoleIds = roleIds
                .Except((await GetCurrentIdentitiesAsync(cancellationToken))
                .Select(x => x.Id))
                .ToArray();

            if (invalidRoleIds.Length == 0)
            {
                RolesLogMessages.RoleIdsValidationSucceeded(_logger);
                return OperationResult.Success;
            }
            else if (invalidRoleIds.Length == 1)
            {
                RolesLogMessages.RoleIdsValidationFailed(_logger, invalidRoleIds);
                return new DataNotFoundError($"Role ID {invalidRoleIds.First()}");
            }
            else
            {
                RolesLogMessages.RoleIdsValidationFailed(_logger, invalidRoleIds);
                return new DataNotFoundError($"Role IDs {string.Join(", ", invalidRoleIds)}");
            }
        }

        private async Task<bool> HandleAddedPermissions(
            IEnumerable<RolePermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            long roleId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedPermissionIds = grantedPermissionIds
                .Except(permissionMappings.Select(x => x.PermissionId))
                .ToArray();

            if (!addedPermissionIds.Any())
                return false;

            RolesLogMessages.RolePermissionMappingsCreating(_logger, roleId, addedPermissionIds);
            var mappingIds = await _rolesRepository.CreatePermissionMappingsAsync(
                roleId,
                addedPermissionIds,
                actionId,
                cancellationToken);
            RolesLogMessages.RolePermissionMappingsCreated(_logger, roleId, mappingIds);

            return true;
        }

        private async Task<bool> HandleRemovedPermissionMappings(
            long roleId,
            IEnumerable<RolePermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedPermissionMappingIds = permissionMappings
                .Where(x => !grantedPermissionIds.Contains(x.PermissionId))
                .Select(x => x.Id)
                .ToArray();

            if (!removedPermissionMappingIds.Any())
                return false;

            RolesLogMessages.RolePermissionMappingsDeleting(_logger, roleId, removedPermissionMappingIds);
            await _rolesRepository.UpdatePermissionMappingsAsync(
                removedPermissionMappingIds,
                deletionId: actionId,
                cancellationToken);
            RolesLogMessages.RolePermissionMappingsDeleted(_logger, roleId);

            return true;
        }

        private async Task<OperationResult> ValidateNameAsync(
            string name,
            long? roleId,
            CancellationToken cancellationToken)
        {
            var nameIsInUse = await _rolesRepository.AnyVersionsAsync(
                excludedRoleIds: roleId?.ToEnumerable()?.ToOptional() ?? default,
                name: name,
                isDeleted: false,
                isLatestVersion: true,
                cancellationToken: cancellationToken);

            return nameIsInUse
                ? new NameInUseError(name)
                : OperationResult.Success;
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IMessenger _messenger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRolesRepository _rolesRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        internal const string _getCurrentIdentitiesCacheKey
            = nameof(RolesService) + "." + nameof(GetCurrentIdentitiesAsync);
    }
}
