using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Business.Users;
using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Data.Authentication;

namespace Sokan.Yastah.Business.Authentication
{
    public interface IAuthenticationService
    {
        AuthenticationTicket CurrentTicket { get; }

        Task<AuthenticationTicket> OnAuthenticatedAsync(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions,
            CancellationToken cancellationToken);

        Task<AuthenticationTicket> OnSignInAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken);
    }

    public class AuthenticationService
        : IAuthenticationService,
            INotificationHandler<RoleUpdatingNotification>,
            INotificationHandler<UserInitializingNotification>,
            INotificationHandler<UserUpdatingNotification>
    {
        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IOptions<AuthorizationConfiguration> authorizationConfiguration,
            IMemoryCache memoryCache,
            ITransactionScopeFactory transactionScopeFactory,
            IUsersService usersService)
        {
            _authenticationRepository = authenticationRepository;
            _authorizationConfiguration = authorizationConfiguration.Value;
            _memoryCache = memoryCache;
            _transactionScopeFactory = transactionScopeFactory;
            _usersService = usersService;
        }

        public AuthenticationTicket CurrentTicket
            => _currentTicket;

        public async Task<AuthenticationTicket> OnAuthenticatedAsync(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions,
            CancellationToken cancellationToken)
        {
            var activeTicketId = await GetActiveTicketId(userId, cancellationToken);

            if (activeTicketId != ticketId)
                grantedPermissions = (await _usersService.GetGrantedPermissionsAsync(
                        userId,
                        cancellationToken))
                    .Value
                    .ToDictionary(x => x.Id, x => x.Name);                    

            _currentTicket = new AuthenticationTicket(
                activeTicketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);

            return _currentTicket;
        }

        public async Task<AuthenticationTicket> OnSignInAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken = default)
        {
            // Don't bother tracking or retrieving permissions for users we don't care about.
            var isAdmin = _authorizationConfiguration.AdminUserIds.Contains(userId);
            if (!isAdmin && !(await IsMemberAsync(getGuildIdsDelegate, cancellationToken)))
                return null;

            await _usersService.TrackUserAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                cancellationToken);

            var ticketId = await GetActiveTicketId(userId, cancellationToken);

            var grantedPermissions = await _usersService.GetGrantedPermissionsAsync(
                userId,
                cancellationToken);

            return new AuthenticationTicket(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions.Value
                    .ToDictionary(x => x.Id, x => x.Name));
        }

        public async Task OnNotificationPublishedAsync(
            RoleUpdatingNotification notification,
            CancellationToken cancellationToken)
        {
            var userIds = await _usersService.GetRoleMemberIdsAsync(
                notification.RoleId,
                cancellationToken);

            foreach(var userId in userIds)
                await UpdateActiveTicketId(
                    userId,
                    notification.ActionId,
                    cancellationToken);
        }

        public Task OnNotificationPublishedAsync(
                UserInitializingNotification notification,
                CancellationToken cancellationToken)
            => UpdateActiveTicketId(
                notification.UserId,
                notification.ActionId,
                cancellationToken);

        public Task OnNotificationPublishedAsync(
                UserUpdatingNotification notification,
                CancellationToken cancellationToken)
            => UpdateActiveTicketId(
                notification.UserId,
                notification.ActionId,
                cancellationToken);

        private async Task<bool> IsMemberAsync(
                Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
                CancellationToken cancellationToken)
            => (await getGuildIdsDelegate.Invoke(cancellationToken))
                .Intersect(_authorizationConfiguration.MemberGuildIds)
                .Any();

        private ValueTask<long> GetActiveTicketId(
                ulong userId,
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(MakeUserActiveTicketIdCacheKey(userId), async entry =>
            {
                entry.Priority = CacheItemPriority.High;

                return (await _authenticationRepository.ReadActiveTicketId(userId, cancellationToken)).Value;
            });

        private async Task UpdateActiveTicketId(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var activeTicketIdResult = await _authenticationRepository.ReadActiveTicketId(userId, cancellationToken);

                if (activeTicketIdResult.IsSuccess)
                    await _authenticationRepository.DeleteTicketAsync(
                        activeTicketIdResult.Value,
                        actionId,
                        cancellationToken);

                var newTicketId = await _authenticationRepository.CreateTicketAsync(
                    userId,
                    actionId,
                    cancellationToken);

                _memoryCache.Set(MakeUserActiveTicketIdCacheKey(userId), newTicketId);

                transactionScope.Complete();
            }
        }

        private static string MakeUserActiveTicketIdCacheKey(ulong userId)
            => $"{nameof(AuthenticationService)}.UserActiveTicketId.{userId}";

        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IUsersService _usersService;

        private AuthenticationTicket _currentTicket;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<AuthenticationService>()
                .AddScoped<IAuthenticationService>(x => x.GetRequiredService<AuthenticationService>())
                .AddScoped<INotificationHandler<RoleUpdatingNotification>>(x => x.GetRequiredService<AuthenticationService>())
                .AddScoped<INotificationHandler<UserInitializingNotification>>(x => x.GetRequiredService<AuthenticationService>())
                .AddScoped<INotificationHandler<UserUpdatingNotification>>(x => x.GetRequiredService<AuthenticationService>());
    }
}
