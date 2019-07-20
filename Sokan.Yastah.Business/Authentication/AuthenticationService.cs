using System;
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
using Sokan.Yastah.Data.Permissions;
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
            IOptions<AuthorizationConfiguration> authorizationConfiguration,
            IAuthorizationService authorizationService,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory,
            IUserRepository userRepository)
        {
            _authorizationConfiguration = authorizationConfiguration.Value;
            _authorizationService = authorizationService;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
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
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {

                // Don't bother tracking or retrieving permissions for users we don't care about.
                var isAdmin = _authorizationConfiguration.AdminUserIds.Contains(userId);
                if (!isAdmin && !(await IsMemberAsync(getGuildIdsDelegate)))
                    return Array.Empty<PermissionIdentity>();

                var now = _systemClock.UtcNow;

                await _userRepository.MergeAsync(
                    userId,
                    username,
                    discriminator,
                    avatarHash,
                    firstSeen: now,
                    lastSeen: now,
                    cancellationToken);

                transactionScope.Complete();
            }

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

        private readonly ISystemClock _systemClock;

        private readonly ITransactionScopeFactory _transactionScopeFactory;

        private readonly IUserRepository _userRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
