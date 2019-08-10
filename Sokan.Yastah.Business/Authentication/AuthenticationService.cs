using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Users;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Authentication
{
    public interface IAuthenticationService
    {
        AuthenticationTicket CurrentTicket { get; }

        void OnAuthenticated(
            AuthenticationTicket authenticationTicket);

        Task<IReadOnlyCollection<PermissionIdentity>> OnSignInAsync(
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
            IOptions<AuthorizationConfiguration> authorizationConfiguration,
            IUsersService usersService)
        {
            _authorizationConfiguration = authorizationConfiguration.Value;
            _usersService = usersService;
        }

        public AuthenticationTicket CurrentTicket
            => _currentTicket;

        public void OnAuthenticated(
                AuthenticationTicket authenticationTicket)
            => _currentTicket = authenticationTicket;

        public async Task<IReadOnlyCollection<PermissionIdentity>> OnSignInAsync(
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

            await _usersService.TrackUserAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                cancellationToken);

            return (await _usersService.GetGrantedPermissionsAsync(
                    userId,
                    cancellationToken))
                .Value;
        }

        private async Task<bool> IsMemberAsync(Func<Task<IEnumerable<ulong>>> getGuildIdsDelegate)
            => (await getGuildIdsDelegate.Invoke())
                .Intersect(_authorizationConfiguration.MemberGuildIds)
                .Any();

        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly IUsersService _usersService;

        private AuthenticationTicket _currentTicket;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
