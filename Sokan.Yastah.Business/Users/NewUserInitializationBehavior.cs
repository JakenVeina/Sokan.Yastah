using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public class NewUserInitializationBehavior
        : INotificationHandler<UserCreatedNotification>
    {
        public NewUserInitializationBehavior(
            IAdministrationActionRepository administrationActionRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory,
            IUserRepository userRepository)
        {
            _administrationActionRepository = administrationActionRepository;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
            _userRepository = userRepository;
        }

        public async Task OnNotificationPublishedAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var actionId = await _administrationActionRepository.CreateAsync(
                    (int)UserManagementAdministrationActionType.UserInitialization,
                    _systemClock.UtcNow,
                    notification.UserId,
                    cancellationToken);

                var defaultPermissionIds = await _userRepository
                    .GetDefaultPermissionIdsAsync(cancellationToken);

                if(defaultPermissionIds.Any())
                    await _userRepository.CreatePermissionMappingsAsync(
                        notification.UserId,
                        defaultPermissionIds,
                        PermissionMappingType.Granted,
                        actionId,
                        cancellationToken);

                var defaultRoleIds = await _userRepository
                    .GetDefaultRoleIdsAsync(cancellationToken);

                if(defaultRoleIds.Any())
                    await _userRepository.CreateRoleMappingsAsync(
                        notification.UserId,
                        defaultRoleIds,
                        actionId,
                        cancellationToken);

                transactionScope.Complete();
            }
        }

        private readonly IAdministrationActionRepository _administrationActionRepository;

        private readonly ISystemClock _systemClock;

        private readonly ITransactionScopeFactory _transactionScopeFactory;

        private readonly IUserRepository _userRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<INotificationHandler<UserCreatedNotification>, NewUserInitializationBehavior>();
    }
}
