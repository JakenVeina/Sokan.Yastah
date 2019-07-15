using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Data.Authorization;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Authentication
{
    public interface IAuthenticationService
    {
        Task<IReadOnlyCollection<PermissionIdentity>> OnUserAuthenticatedAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken);
    }

    public class AuthenticationService
        : IAuthenticationService
    {
        public AuthenticationService(
            AuthorizationConfiguration authorizationConfiguration,
            IAuthorizationService authorizationService,
            IUserRepository userRepository)
        {
            _authorizationConfiguration = authorizationConfiguration;
            _authorizationService = authorizationService;
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyCollection<PermissionIdentity>> OnUserAuthenticatedAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken = default)
        {
            // Don't bother tracking or retrieving permissions for users we don't care about.
            var isAdmin = _authorizationConfiguration.AdminUserIds.Contains(userId);
            if (!isAdmin && !(await IsMemberAsync(getGuildIdsDelegate)))
                return Array.Empty<PermissionIdentity>();

            await _userRepository.MergeAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                cancellationToken);

            return await _authorizationService.GetUserGrantedPermissionsAsync(
                userId,
                cancellationToken);
        }

        private async Task<bool> IsMemberAsync(Func<Task<IEnumerable<ulong>>> getGuildIdsDelegate)
            => (await getGuildIdsDelegate.Invoke())
                .Intersect(_authorizationConfiguration.MemberGuildIds)
                .Any();

        private readonly AuthorizationConfiguration _authorizationConfiguration;

        private readonly IAuthorizationService _authorizationService;

        private readonly IUserRepository _userRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
