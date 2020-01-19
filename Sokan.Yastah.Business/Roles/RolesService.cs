using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

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

    public class RolesService
        : IRolesService
    {
        public RolesService(
            IAdministrationActionsRepository administrationActionsRepository,
            IMemoryCache memoryCache,
            IMessenger messenger,
            IPermissionsService permissionsService,
            IRolesRepository rolesRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
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
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var nameValidationResult = await ValidateNameAsync(creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult.Error;

            var grantedPermissionIdsValidationResult = await _permissionsService.ValidateIdsAsync(creationModel.GrantedPermissionIds, cancellationToken);
            if (grantedPermissionIdsValidationResult.IsFailure)
                return grantedPermissionIdsValidationResult.Error;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var roleId = await _rolesRepository.CreateAsync(
                creationModel.Name,
                actionId,
                cancellationToken);

            await _rolesRepository.CreatePermissionMappingsAsync(
                roleId,
                creationModel.GrantedPermissionIds,
                actionId,
                cancellationToken);

            _memoryCache.Remove(_getCurrentIdentitiesCacheKey);

            transactionScope.Complete();

            return roleId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
            long roleId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var updateResult = await _rolesRepository.UpdateAsync(
                roleId: roleId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if(updateResult.IsSuccess)
            {
                transactionScope.Complete();
                _memoryCache.Remove(_getCurrentIdentitiesCacheKey);
            }

            return updateResult;
        }

        public ValueTask<IReadOnlyCollection<RoleIdentityViewModel>> GetCurrentIdentitiesAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(_getCurrentIdentitiesCacheKey, async entry =>
            {
                entry.Priority = CacheItemPriority.High;

                return await _rolesRepository.AsyncEnumerateIdentities(
                            isDeleted: false)
                        .ToArrayAsync(cancellationToken)
                    as IReadOnlyCollection<RoleIdentityViewModel>;
            });

        public async Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var nameValidationResult = await ValidateNameAsync(updateModel.Name, roleId, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult;

            var grantedPermissionIdsValidationResult = await _permissionsService.ValidateIdsAsync(updateModel.GrantedPermissionIds, cancellationToken);
            if (grantedPermissionIdsValidationResult.IsFailure)
                return grantedPermissionIdsValidationResult;

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)RoleManagementAdministrationActionType.RoleModified,
                now,
                performedById,
                cancellationToken);

            var updateResult = await _rolesRepository.UpdateAsync(
                roleId: roleId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);
            if (updateResult.IsFailure && !(updateResult.Error is NoChangesGivenError))
                return updateResult;

            var anyChanges = updateResult.IsSuccess;

            var permissionMappings = await _rolesRepository.AsyncEnumeratePermissionMappingIdentities(
                    roleId: roleId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);

            anyChanges |= await HandleRemovedPermissionMappings(
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
                return new NoChangesGivenError($"Role ID {roleId}");

            await _messenger.PublishNotificationAsync(
                new RoleUpdatingNotification(
                    roleId,
                    actionId),
                cancellationToken);

            transactionScope.Complete();

            _memoryCache.Remove(_getCurrentIdentitiesCacheKey);

            return OperationResult.Success;
        }

        public async ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<long> roleIds,
            CancellationToken cancellationToken)
        {
            if (!roleIds.Any())
                return OperationResult.Success;

            var invalidRoleIds = roleIds
                .Except((await GetCurrentIdentitiesAsync(cancellationToken))
                .Select(x => x.Id)).ToArray();

            return invalidRoleIds.Any()
                ? ((invalidRoleIds.Length == 1)
                    ? new DataNotFoundError($"Role ID {invalidRoleIds.First()}")
                    : new DataNotFoundError($"Role IDs {string.Join(", ", invalidRoleIds)}"))
                : OperationResult.Success;
        }

        private async Task<bool> HandleAddedPermissions(
            IEnumerable<RolePermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            long roleId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedPermissionIds = grantedPermissionIds
                .Except(permissionMappings.Select(x => x.PermissionId));

            if (!addedPermissionIds.Any())
                return false;

            await _rolesRepository.CreatePermissionMappingsAsync(
                roleId,
                addedPermissionIds,
                actionId,
                cancellationToken);

            return true;
        }

        private async Task<bool> HandleRemovedPermissionMappings(
            IEnumerable<RolePermissionMappingIdentityViewModel> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedPermissionMappingIds = permissionMappings
                .Where(x => !grantedPermissionIds.Contains(x.PermissionId))
                .Select(x => x.Id);

            if (!removedPermissionMappingIds.Any())
                return false;

            await _rolesRepository.UpdatePermissionMappingsAsync(
                removedPermissionMappingIds,
                deletionId: actionId,
                cancellationToken);

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
        private readonly IMemoryCache _memoryCache;
        private readonly IMessenger _messenger;
        private readonly IPermissionsService _permissionsService;
        private readonly IRolesRepository _rolesRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        internal const string _getCurrentIdentitiesCacheKey
            = nameof(RolesService) + "." + nameof(GetCurrentIdentitiesAsync);

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<IRolesService, RolesService>();
    }
}
