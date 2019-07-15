using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Data.Authorization;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Authorization
{
    public interface IAuthorizationService
    {
        Task<IReadOnlyCollection<PermissionIdentity>> GetUserGrantedPermissionsAsync(
            ulong userId,
            CancellationToken cancellationToken);
    }

    public class AuthorizationService
        : IAuthorizationService
    {
        public AuthorizationService(
            AuthorizationConfiguration authorizationConfiguration,
            IPermissionRepository permissionRepository,
            IUserRepository userRepository)
        {
            _authorizationConfiguration = authorizationConfiguration;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }

        public Task<IReadOnlyCollection<PermissionIdentity>> GetUserGrantedPermissionsAsync(
                ulong userId,
                CancellationToken cancellationToken)
            => _authorizationConfiguration.AdminUserIds.Contains(userId)
                ? _permissionRepository.GetAllPermissionIdentitiesAsync(cancellationToken)
                : _userRepository.GetGrantedPermissionIdentitiesAsync(userId, cancellationToken);

        private readonly AuthorizationConfiguration _authorizationConfiguration;

        private readonly IPermissionRepository _permissionRepository;

        private readonly IUserRepository _userRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IAuthorizationService, AuthorizationService>();
    }
}
