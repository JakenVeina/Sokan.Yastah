using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Data.Authorization;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public class NewUserInitializationBehavior
        : INotificationHandler<UserCreatedNotification>
    {
        public NewUserInitializationBehavior(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task OnNotificationPublishedAsync(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            var defaultPermissionIds = await _userRepository
                .GetDefaultPermissionIdsAsync(cancellationToken);

            if(defaultPermissionIds.Any())
                await _userRepository.CreatePermissionMappingsAsync(
                    notification.UserId,
                    defaultPermissionIds,
                    PermissionMappingType.Granted,
                    notification.UserId,
                    cancellationToken);

            var defaultRoleIds = await _userRepository
                .GetDefaultRoleIdsAsync(cancellationToken);

            if(defaultRoleIds.Any())
                await _userRepository.CreateRoleMappingsAsync(
                    notification.UserId,
                    defaultRoleIds,
                    notification.UserId,
                    cancellationToken);
        }

        private readonly IUserRepository _userRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<INotificationHandler<UserCreatedNotification>, NewUserInitializationBehavior>();
    }
}
