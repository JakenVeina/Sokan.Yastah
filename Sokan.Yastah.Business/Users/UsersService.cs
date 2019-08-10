using System.Collections.Generic;
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

                return (await _usersRepository.GetGrantedPermissionIdentitiesAsync(userId, cancellationToken))
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
                        .GetDefaultPermissionIdsAsync(cancellationToken);

                    if (defaultPermissionIds.Any())
                        await _usersRepository.CreatePermissionMappingsAsync(
                            userId,
                            defaultPermissionIds,
                            PermissionMappingType.Granted,
                            actionId,
                            cancellationToken);

                    var defaultRoleIds = await _usersRepository
                        .GetDefaultRoleIdsAsync(cancellationToken);

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
