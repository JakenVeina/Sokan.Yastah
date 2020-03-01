using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Authentication
{
    internal static class AuthenticationLogMessages
    {
        public static void UserIgnored(
                ILogger logger,
                ulong userId,
                string username,
                string discriminator)
            => _userIgnored.Invoke(
                logger,
                userId,
                username,
                discriminator);
        private static readonly Action<ILogger, ulong, string, string> _userIgnored
            = LoggerMessage.Define<ulong, string, string>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(UserIgnored)),
                    "User is neither admin or member: ignoring:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
                .WithoutException();
        
        public static void UserSigningIn(
                ILogger logger,
                ulong userId,
                string username,
                string discriminator)
            => _userSigningIn.Invoke(
                logger,
                userId,
                username,
                discriminator);
        private static readonly Action<ILogger, ulong, string, string> _userSigningIn
            = LoggerMessage.Define<ulong, string, string>(
                    LogLevel.Information,
                    new EventId(3001, nameof(UserSigningIn)),
                    "User signing in:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
                .WithoutException();

        public static void UserSignedIn(
                ILogger logger,
                long ticketId,
                ulong userId,
                string username,
                string discriminator)
            => _userSignedIn.Invoke(
                logger,
                ticketId,
                userId,
                username,
                discriminator);
        private static readonly Action<ILogger, long, ulong, string, string> _userSignedIn
            = LoggerMessage.Define<long, ulong, string, string>(
                    LogLevel.Information,
                    new EventId(3002, nameof(UserSignedIn)),
                    "User signed in:\r\n\tTicketId: {TicketId}\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
                .WithoutException();

        public static void AuthenticationTicketsInvalidating(
                ILogger logger,
                long roleId)
            => _authenticationTicketsInvalidating.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _authenticationTicketsInvalidating
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3003, nameof(AuthenticationTicketsInvalidating)),
                    "Invalidating authentication tickets:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void AuthenticationTicketsInvalidated(
                ILogger logger,
                long roleId)
            => _authenticationTicketsInvalidated.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _authenticationTicketsInvalidated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3004, nameof(AuthenticationTicketsInvalidated)),
                    "Authentication tickets invalidated:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void AuthenticationTicketInvalidating(
                ILogger logger,
                ulong userId,
                long actionId)
            => _authenticationTicketInvalidating.Invoke(
                logger,
                userId,
                actionId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketInvalidating
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Information,
                    new EventId(3005, nameof(AuthenticationTicketInvalidating)),
                    "Invalidating authentication ticket:\r\n\tUserId: {UserId}\r\n\tActionId: {ActionId}")
                .WithoutException();

        public static void AuthenticationTicketInvalidated(
                ILogger logger,
                ulong userId,
                long ticketId)
            => _authenticationTicketInvalidated.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketInvalidated
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Information,
                    new EventId(3006, nameof(AuthenticationTicketInvalidated)),
                    "Authentication ticket invalidated:\r\n\tUserId: {UserId}\r\n\tTicketId: {TicketId}")
                .WithoutException();

        public static void AuthenticationTicketAssembling(
                ILogger logger,
                long ticketId,
                ulong userId,
                string username,
                string discriminator,
                string avatarHash,
                IReadOnlyDictionary<int, string> grantedPermissions)
            => _authenticationTicketAssembling.Invoke(
                logger,
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);
        private static readonly Action<ILogger, long, ulong, string, string, string, IReadOnlyDictionary<int, string>> _authenticationTicketAssembling
            = LoggerMessage.Define<long, ulong, string, string, string, IReadOnlyDictionary<int, string>>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(AuthenticationTicketAssembling)),
                    $"Assembling {nameof(AuthenticationTicket)}:\r\n\tTicketId: {{TicketId}}\r\n\tUserId: {{UserId}}\r\n\tUsername: {{Username}}\r\n\tDiscriminator: {{Discriminator}}\r\n\tAvatarHash: {{AvatarHash}}\r\n\tGrantedPermissions: {{GrantedPermissions}}")
                .WithoutException();

        public static void AuthenticationTicketAssembled(
                ILogger logger,
                long ticketId)
            => _authenticationTicketAssembled.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketAssembled
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(AuthenticationTicketAssembled)),
                    $"{nameof(AuthenticationTicket)} assembled: TicketId: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketRebuilding(
                ILogger logger,
                long oldTicketId,
                long newTicketId)
            => _authenticationTicketRebuilding.Invoke(
                logger,
                oldTicketId,
                newTicketId);
        private static readonly Action<ILogger, long, long> _authenticationTicketRebuilding
            = LoggerMessage.Define<long, long>(
                    LogLevel.Debug,
                    new EventId(4003, nameof(AuthenticationTicketRebuilding)),
                    $"Rebuilding {nameof(AuthenticationTicket)}:\r\n\tOldTicketId: {{OldTicketId}}\r\n\tNewTicketId: {{NewTicketId}}")
                .WithoutException();

        public static void AuthenticationTicketActiveIdFetched(
                ILogger logger,
                ulong userId,
                long? ticketId)
            => _authenticationTicketActiveIdFetched.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long?> _authenticationTicketActiveIdFetched
            = LoggerMessage.Define<ulong, long?>(
                    LogLevel.Debug,
                    new EventId(4004, nameof(AuthenticationTicketActiveIdFetched)),
                    $"{nameof(AuthenticationTicket)} active ID fetched:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketCreating(
                ILogger logger,
                ulong userId)
            => _authenticationTicketCreating.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _authenticationTicketCreating
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4005, nameof(AuthenticationTicketCreating)),
                    $"Creating new {nameof(AuthenticationTicket)}: UserID: {{UserId}}")
                .WithoutException();

        public static void AuthenticationTicketCreated(
                ILogger logger,
                ulong userId,
                long ticketId)
            => _authenticationTicketCreated.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketCreated
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Debug,
                    new EventId(4006, nameof(AuthenticationTicketCreated)),
                    $"New {nameof(AuthenticationTicket)} created:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketDeleting(
                ILogger logger,
                ulong userId,
                long ticketId)
            => _authenticationTicketDeleting.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketDeleting
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Debug,
                    new EventId(4007, nameof(AuthenticationTicketDeleting)),
                    $"Deleting {nameof(AuthenticationTicket)}:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketDeleted(
                ILogger logger,
                ulong userId,
                long ticketId)
            => _authenticationTicketDeleted.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketDeleted
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Debug,
                    new EventId(4008, nameof(AuthenticationTicketDeleted)),
                    $"{nameof(AuthenticationTicket)} deleted:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
                .WithoutException();

        public static void UserInitializing(
                ILogger logger,
                ulong userId)
            => _userInitializing.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userInitializing
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4009, nameof(UserInitializing)),
                    "Initializing user: {UserId}")
                .WithoutException();

        public static void UserUpdating(
                ILogger logger,
                ulong userId)
            => _userUpdating.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userUpdating
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4010, nameof(UserUpdating)),
                    "Updating user: {UserId}")
                .WithoutException();

        public static void UserTracked(
                ILogger logger,
                ulong userId)
            => _userTracked.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _userTracked
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4011, nameof(UserTracked)),
                    "User tracked: {UserId}")
                .WithoutException();

        public static void GrantedPermissionsFetched(
                ILogger logger,
                ulong userId)
            => _grantedPermissionsFetched.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _grantedPermissionsFetched
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4012, nameof(GrantedPermissionsFetched)),
                    "Granted Permissions fetched: {UserId}")
                .WithoutException();

        public static void RoleUpdating(
                ILogger logger,
                long roleId)
            => _roleUpdating.Invoke(
                logger,
                roleId);
        private static readonly Action<ILogger, long> _roleUpdating
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4013, nameof(RoleUpdating)),
                    "Updating role: {RoleId}")
                .WithoutException();

        public static void GuildIdsFetching(
                ILogger logger,
                ulong userId)
            => _guildIdsFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _guildIdsFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4014, nameof(GuildIdsFetching)),
                    "Fetching User Guild IDs: UserId: {UserId}")
                .WithoutException();

        public static void GuildIdsFetched(
                ILogger logger,
                ulong userId)
            => _guildIdsFetched.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _guildIdsFetched
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4015, nameof(GuildIdsFetched)),
                    "User Guild IDs fetched: UserId: {UserId}")
                .WithoutException();
    }
}
