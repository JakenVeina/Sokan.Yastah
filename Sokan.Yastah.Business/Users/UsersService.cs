﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public interface IUsersService
    {
        Task<OperationResult<IReadOnlyCollection<PermissionIdentity>>> GetGrantedPermissionsAsync(
            ulong userId,
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

    public class UsersService
        : IUsersService
    {
        public UsersService(
            IAdministrationActionsRepository administrationActionsRepository,
            IOptions<AuthorizationConfiguration> authorizationConfigurationOptions,
            IPermissionsService permissionsService,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory,
            IUsersRepository usersRepository)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _authorizationConfigurationOptions = authorizationConfigurationOptions;
            _permissionsService = permissionsService;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
            _usersRepository = usersRepository;
        }

        public async Task<OperationResult<IReadOnlyCollection<PermissionIdentity>>> GetGrantedPermissionsAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                if (_authorizationConfigurationOptions.Value.AdminUserIds.Contains(userId))
                    return (await _permissionsService.GetIdentitiesAsync(cancellationToken))
                        .ToSuccess();

                var userExists = await _usersRepository.AnyAsync(
                    userId: userId,
                    cancellationToken: cancellationToken);

                if (!userExists)
                    return new DataNotFoundError($"User ID {userId}")
                        .ToError<IReadOnlyCollection<PermissionIdentity>>();

                return (await _usersRepository.ReadGrantedPermissionIdentitiesAsync(userId, cancellationToken))
                    .ToSuccess();
            }
        }

        public async Task TrackUserAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var now = _systemClock.UtcNow;

                var result = await _usersRepository.MergeAsync(
                    userId,
                    username,
                    discriminator,
                    avatarHash,
                    firstSeen: now,
                    lastSeen: now,
                    cancellationToken);

                if(result.RowsInserted > 0)
                {
                    var actionId = await _administrationActionsRepository.CreateAsync(
                        (int)UserManagementAdministrationActionType.UserInitialization,
                        now,
                        userId,
                        cancellationToken);

                    var defaultPermissionIds = await _usersRepository
                        .ReadDefaultPermissionIdsAsync(cancellationToken);

                    if (defaultPermissionIds.Any())
                        await _usersRepository.CreatePermissionMappingsAsync(
                            userId,
                            defaultPermissionIds,
                            PermissionMappingType.Granted,
                            actionId,
                            cancellationToken);

                    var defaultRoleIds = await _usersRepository
                        .ReadDefaultRoleIdsAsync(cancellationToken);

                    if (defaultRoleIds.Any())
                        await _usersRepository.CreateRoleMappingsAsync(
                            userId,
                            defaultRoleIds,
                            actionId,
                            cancellationToken);
                }

                transactionScope.Complete();
            }
        }

        public async Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var now = _systemClock.UtcNow;

                var actionId = await _administrationActionsRepository.CreateAsync(
                    (int)UserManagementAdministrationActionType.UserInitialization,
                    now,
                    performedById,
                    cancellationToken);

                var anyChanges = false;

                var permissionMappings = await _usersRepository.ReadPermissionMappingIdentitiesAsync(
                    userId: userId,
                    isDeleted: false,
                    cancellationToken: cancellationToken);

                anyChanges |= await HandleRemovedPermissionMappings(
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

                var roleMappings = await _usersRepository.ReadRoleMappingIdentitiesAsync(
                    userId: userId,
                    isDeleted: false,
                    cancellationToken: cancellationToken);

                anyChanges |= await HandleRemovedRoleMappings(
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

                if(!anyChanges)
                    return new NoChangesGivenError($"User ID {userId}")
                        .ToError();

                transactionScope.Complete();

                return OperationResult.Success;
            }
        }

        private async Task<bool> HandleAddedDeniedPermissions(
            IEnumerable<UserPermissionMappingIdentity> permissionMappings,
            IEnumerable<int> deniedPermissionIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedDeniedPermissionIds = deniedPermissionIds
                .Where(id => !permissionMappings.Any(x => x.isDenied && (x.PermissionId == id)))
                .ToArray();

            if (!addedDeniedPermissionIds.Any())
                return false;

            await _usersRepository.CreatePermissionMappingsAsync(
                userId,
                addedDeniedPermissionIds,
                PermissionMappingType.Denied,
                actionId,
                cancellationToken);

            return true;
        }

        private async Task<bool> HandleAddedGrantedPermissions(
            IEnumerable<UserPermissionMappingIdentity> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedGrantedPermissionIds = grantedPermissionIds
                .Where(id => !permissionMappings.Any(x => !x.isDenied && (x.PermissionId == id)))
                .ToArray();

            if (!addedGrantedPermissionIds.Any())
                return false;

            await _usersRepository.CreatePermissionMappingsAsync(
                userId,
                addedGrantedPermissionIds,
                PermissionMappingType.Granted,
                actionId,
                cancellationToken);

            return true;
        }

        private async Task<bool> HandleAddedRoles(
            IEnumerable<UserRoleMappingIdentity> roleMappings,
            IEnumerable<long> assignedRoleIds,
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            var addedRoleMappingIds = assignedRoleIds
                .Where(id => !roleMappings.Any(x => x.RoleId == id))
                .ToArray();

            if (!addedRoleMappingIds.Any())
                return false;

            await _usersRepository.CreateRoleMappingsAsync(
                userId,
                addedRoleMappingIds,
                actionId,
                cancellationToken);

            return true;
        }

        private async Task<bool> HandleRemovedPermissionMappings(
            IEnumerable<UserPermissionMappingIdentity> permissionMappings,
            IEnumerable<int> grantedPermissionIds,
            IEnumerable<int> deniedPermissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedPermissionMappingIds = permissionMappings
                .Where(x => (!x.isDenied && !grantedPermissionIds.Contains(x.PermissionId))
                    || (x.isDenied && !deniedPermissionIds.Contains(x.PermissionId)))
                .Select(x => x.Id);

            if (!removedPermissionMappingIds.Any())
                return false;

            await _usersRepository.UpdatePermissionMappingsAsync(
                removedPermissionMappingIds,
                actionId,
                cancellationToken);

            return true;
        }

        private async Task<bool> HandleRemovedRoleMappings(
            IEnumerable<UserRoleMappingIdentity> roleMappings,
            IEnumerable<long> assignedRoleIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var removedRoleMappingIds = roleMappings
                .Where(x => !assignedRoleIds.Contains(x.RoleId))
                .Select(x => x.Id)
                .ToArray();

            if (!removedRoleMappingIds.Any())
                return false;

            await _usersRepository.UpdateRoleMappingsAsync(
                removedRoleMappingIds,
                actionId,
                cancellationToken);

            return true;
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly IOptions<AuthorizationConfiguration> _authorizationConfigurationOptions;
        private readonly IPermissionsService _permissionsService;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IUsersRepository _usersRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IUsersService, UsersService>();
    }
}