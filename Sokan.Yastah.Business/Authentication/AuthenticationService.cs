using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        AuthenticationTicket? CurrentTicket { get; }

        Task<AuthenticationTicket> OnAuthenticatedAsync(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions,
            CancellationToken cancellationToken);

        Task<AuthenticationTicket?> OnSignInAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class AuthenticationService
        : IAuthenticationService,
            INotificationHandler<RoleUpdatingNotification>,
            INotificationHandler<UserInitializingNotification>,
            INotificationHandler<UserUpdatingNotification>
    {
        public AuthenticationService(
            IAuthenticationTicketsRepository authenticationTicketsRepository,
            IOptions<AuthorizationConfiguration> authorizationConfigurationOptions,
            ILogger<AuthenticationService> logger,
            IMemoryCache memoryCache,
            ITransactionScopeFactory transactionScopeFactory,
            IUsersService usersService)
        {
            _authenticationTicketsRepository = authenticationTicketsRepository;
            _authorizationConfiguration = authorizationConfigurationOptions.Value;
            _logger = logger;
            _memoryCache = memoryCache;
            _transactionScopeFactory = transactionScopeFactory;
            _usersService = usersService;
        }

        public AuthenticationTicket? CurrentTicket
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
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.AuthenticationPerforming(_logger, ticketId, userId, username, discriminator, avatarHash, grantedPermissions);

            AuthenticationLogMessages.AuthenticationTicketActiveIdFetching(_logger, userId);
            var activeTicketId = await GetActiveTicketIdAsync(userId, cancellationToken);
            AuthenticationLogMessages.AuthenticationTicketActiveIdFetched(_logger, activeTicketId);

            if (activeTicketId != ticketId)
            {
                AuthenticationLogMessages.GrantedPermissionsFetching(_logger, userId);
                grantedPermissions = (await _usersService.GetGrantedPermissionsAsync(
                        userId,
                        cancellationToken))
                    .Value
                    .ToDictionary(x => x.Id, x => x.Name);
                AuthenticationLogMessages.GrantedPermissionsFetched(_logger, grantedPermissions);
            }

            _currentTicket = new AuthenticationTicket(
                activeTicketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);
            AuthenticationLogMessages.AuthenticationPerformed(_logger, activeTicketId);

            return _currentTicket;
        }

        public async Task<AuthenticationTicket?> OnSignInAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthenticationLogMessages.UserSigningIn(_logger, userId, username, discriminator);

            // Don't bother tracking or retrieving permissions for users we don't care about.
            var isAdmin = _authorizationConfiguration.AdminUserIds.Contains(userId);
            if (!isAdmin && !(await IsMemberAsync(userId, getGuildIdsDelegate, cancellationToken)))
            {
                AuthenticationLogMessages.UserIgnored(_logger, userId, username, discriminator);
                return null;
            }

            await _usersService.TrackUserAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                cancellationToken);
            AuthenticationLogMessages.UserTracked(_logger, userId);

            AuthenticationLogMessages.AuthenticationTicketActiveIdFetching(_logger, userId);
            var ticketId = await GetActiveTicketIdAsync(userId, cancellationToken);
            AuthenticationLogMessages.AuthenticationTicketActiveIdFetched(_logger, ticketId);

            AuthenticationLogMessages.GrantedPermissionsFetching(_logger, userId);
            var grantedPermissions = (await _usersService.GetGrantedPermissionsAsync(
                    userId,
                    cancellationToken))
                .Value
                .ToDictionary(x => x.Id, x => x.Name);

            AuthenticationLogMessages.GrantedPermissionsFetched(_logger, grantedPermissions);

            var ticket = new AuthenticationTicket(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);

            AuthenticationLogMessages.UserSignedIn(_logger, ticketId, userId, username, discriminator);
            return ticket;
        }

        public async Task OnNotificationPublishedAsync(
            RoleUpdatingNotification notification,
            CancellationToken cancellationToken)
        {
            AuthenticationLogMessages.RoleUpdating(_logger, notification.RoleId);
            
            var userIds = await _usersService.GetRoleMemberIdsAsync(
                notification.RoleId,
                cancellationToken);
            AuthenticationLogMessages.AuthenticationTicketsInvalidating(_logger, notification.RoleId);

            foreach(var userId in userIds)
                await UpdateActiveTicketId(
                    userId,
                    notification.ActionId,
                    cancellationToken);

            AuthenticationLogMessages.AuthenticationTicketsInvalidated(_logger, notification.RoleId);
        }

        public Task OnNotificationPublishedAsync(
                UserInitializingNotification notification,
                CancellationToken cancellationToken)
        {
            AuthenticationLogMessages.UserInitializing(_logger, notification.UserId);

            return UpdateActiveTicketId(
                notification.UserId,
                notification.ActionId,
                cancellationToken);
        }

        public Task OnNotificationPublishedAsync(
                UserUpdatingNotification notification,
                CancellationToken cancellationToken)
        {
            AuthenticationLogMessages.UserUpdating(_logger, notification.UserId);

            return UpdateActiveTicketId(
                notification.UserId,
                notification.ActionId,
                cancellationToken);
        }

        private async Task<bool> IsMemberAsync(
                ulong userId,
                Func<CancellationToken, Task<IEnumerable<ulong>>> getGuildIdsDelegate,
                CancellationToken cancellationToken)
        {
            AuthenticationLogMessages.GuildIdsFetching(_logger, userId);
            var guildIds = await getGuildIdsDelegate.Invoke(cancellationToken);
            AuthenticationLogMessages.GuildIdsFetched(_logger, userId);

            return guildIds
                .Intersect(_authorizationConfiguration.MemberGuildIds)
                .Any();
        }

        private ValueTask<long> GetActiveTicketIdAsync(
                ulong userId,
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(MakeUserActiveTicketIdCacheKey(userId), async entry =>
            {
                entry.Priority = CacheItemPriority.High;

                AuthenticationLogMessages.AuthenticationTicketActiveIdFetching(_logger, userId);
                var result = await _authenticationTicketsRepository.ReadActiveIdAsync(userId, cancellationToken);
                AuthenticationLogMessages.AuthenticationTicketActiveIdFetched(_logger, result.Value);

                return result.Value;
            });

        private async Task UpdateActiveTicketId(
            ulong userId,
            long actionId,
            CancellationToken cancellationToken)
        {
            AuthenticationLogMessages.AuthenticationTicketInvalidating(_logger, userId, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

                AuthenticationLogMessages.AuthenticationTicketActiveIdFetching(_logger, userId);
            var activeTicketIdResult = await _authenticationTicketsRepository.ReadActiveIdAsync(userId, cancellationToken);

            if (activeTicketIdResult.IsSuccess)
            {
                AuthenticationLogMessages.AuthenticationTicketActiveIdFetched(_logger, activeTicketIdResult.Value);
                AuthenticationLogMessages.AuthenticationTicketDeleting(_logger, userId, activeTicketIdResult.Value);
                await _authenticationTicketsRepository.DeleteAsync(
                    activeTicketIdResult.Value,
                    actionId,
                    cancellationToken);
                AuthenticationLogMessages.AuthenticationTicketDeleted(_logger, userId, activeTicketIdResult.Value);
            }
            else
            {
                AuthenticationLogMessages.AuthenticationTicketActiveIdFetched(_logger, null);
            }

            AuthenticationLogMessages.AuthenticationTicketCreating(_logger, userId);
            var newTicketId = await _authenticationTicketsRepository.CreateAsync(
                userId,
                actionId,
                cancellationToken);
            AuthenticationLogMessages.AuthenticationTicketCreated(_logger, userId, newTicketId);
            
            _memoryCache.Set(MakeUserActiveTicketIdCacheKey(userId), newTicketId);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            AuthenticationLogMessages.AuthenticationTicketInvalidated(_logger, userId, newTicketId);
        }

        internal static string MakeUserActiveTicketIdCacheKey(ulong userId)
            => $"{nameof(AuthenticationService)}.UserActiveTicketId.{userId}";

        private readonly IAuthenticationTicketsRepository _authenticationTicketsRepository;
        private readonly AuthorizationConfiguration _authorizationConfiguration;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IUsersService _usersService;

        private AuthenticationTicket? _currentTicket;
    }
}
