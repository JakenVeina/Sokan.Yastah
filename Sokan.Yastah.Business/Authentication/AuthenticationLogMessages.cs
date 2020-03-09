using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Business.Authentication
{
    internal static class AuthenticationLogMessages
    {
        public enum EventType
        {
            AuthenticationPerforming                = BusinessLogEventType.Authentication + 0x0001,
            AuthenticationPerformed                 = BusinessLogEventType.Authentication + 0x0002,
            UserSigningIn                           = BusinessLogEventType.Authentication + 0x0003,
            UserSignedIn                            = BusinessLogEventType.Authentication + 0x0004,
            GuildIdsFetching                        = BusinessLogEventType.Authentication + 0x0005,
            GuildIdsFetched                         = BusinessLogEventType.Authentication + 0x0006,
            UserIgnored                             = BusinessLogEventType.Authentication + 0x0007,
            UserTracked                             = BusinessLogEventType.Authentication + 0x0008,
            AuthenticationTicketActiveIdFetching    = BusinessLogEventType.Authentication + 0x0009,
            AuthenticationTicketActiveIdFetched     = BusinessLogEventType.Authentication + 0x000A,
            GrantedPermissionsFetching              = BusinessLogEventType.Authentication + 0x000B,
            GrantedPermissionsFetched               = BusinessLogEventType.Authentication + 0x000C,
            RoleUpdating                            = BusinessLogEventType.Authentication + 0x000D,
            UserInitializing                        = BusinessLogEventType.Authentication + 0x000E,
            UserUpdating                            = BusinessLogEventType.Authentication + 0x000F,
            AuthenticationTicketsInvalidating       = BusinessLogEventType.Authentication + 0x0010,
            AuthenticationTicketsInvalidated        = BusinessLogEventType.Authentication + 0x0011,
            AuthenticationTicketInvalidating        = BusinessLogEventType.Authentication + 0x0012,
            AuthenticationTicketInvalidated         = BusinessLogEventType.Authentication + 0x0013,
            AuthenticationTicketCreating            = BusinessLogEventType.Authentication + 0x0014,
            AuthenticationTicketCreated             = BusinessLogEventType.Authentication + 0x0015,
            AuthenticationTicketDeleting            = BusinessLogEventType.Authentication + 0x0016,
            AuthenticationTicketDeleted             = BusinessLogEventType.Authentication + 0x0017
        }

        public static void AuthenticationPerformed(
                ILogger logger,
                long ticketId)
            => _authenticationPerformed.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationPerformed
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.AuthenticationPerformed.ToEventId(),
                    "Authentication performed: TicketId: {TicketId}")
                .WithoutException();

        public static void AuthenticationPerforming(
                ILogger logger,
                long ticketId,
                ulong userId,
                string username,
                string discriminator,
                string avatarHash,
                IReadOnlyDictionary<int, string> grantedPermissions)
            => _authenticationPerforming.Invoke(
                logger,
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);
        private static readonly Action<ILogger, long, ulong, string, string, string, IReadOnlyDictionary<int, string>> _authenticationPerforming
            = LoggerMessage.Define<long, ulong, string, string, string, IReadOnlyDictionary<int, string>>(
                    LogLevel.Debug,
                    EventType.AuthenticationPerforming.ToEventId(),
                    "Performing authentication:\r\n\tTicketId: {TicketId}\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}\r\n\tAvatarHash: {AvatarHash}\r\n\tGrantedPermissions: {GrantedPermissions}")
                .WithoutException();

        public static void AuthenticationTicketActiveIdFetched(
                ILogger logger,
                long? ticketId)
            => _authenticationTicketActiveIdFetched.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long?> _authenticationTicketActiveIdFetched
            = LoggerMessage.Define<long?>(
                    LogLevel.Debug,
                    EventType.AuthenticationTicketActiveIdFetched.ToEventId(),
                    $"{nameof(AuthenticationTicket)} active ID fetched:\r\n\tTicketId: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketActiveIdFetching(
                ILogger logger,
                ulong userId)
            => _authenticationTicketActiveIdFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _authenticationTicketActiveIdFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.AuthenticationTicketActiveIdFetching.ToEventId(),
                    $"Fetching {nameof(AuthenticationTicket)} active ID:\r\n\tUserId: {{UserId}}")
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
                    EventType.AuthenticationTicketCreated.ToEventId(),
                    $"New {nameof(AuthenticationTicket)} created:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
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
                    EventType.AuthenticationTicketCreating.ToEventId(),
                    $"Creating new {nameof(AuthenticationTicket)}: UserID: {{UserId}}")
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
                    EventType.AuthenticationTicketDeleted.ToEventId(),
                    $"{nameof(AuthenticationTicket)} deleted:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
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
                    EventType.AuthenticationTicketDeleting.ToEventId(),
                    $"Deleting {nameof(AuthenticationTicket)}:\r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
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
                    EventType.AuthenticationTicketInvalidated.ToEventId(),
                    "Authentication ticket invalidated:\r\n\tUserId: {UserId}\r\n\tTicketId: {TicketId}")
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
                    EventType.AuthenticationTicketInvalidating.ToEventId(),
                    "Invalidating authentication ticket:\r\n\tUserId: {UserId}\r\n\tActionId: {ActionId}")
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
                    EventType.AuthenticationTicketsInvalidated.ToEventId(),
                    "Authentication tickets invalidated:\r\n\tRoleId: {RoleId}")
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
                    EventType.AuthenticationTicketsInvalidating.ToEventId(),
                    "Invalidating authentication tickets:\r\n\tRoleId: {RoleId}")
                .WithoutException();

        public static void GrantedPermissionsFetched(
                ILogger logger,
                IReadOnlyDictionary<int, string> grantedPermissions)
            => _grantedPermissionsFetched.Invoke(
                logger,
                grantedPermissions);
        private static readonly Action<ILogger, IReadOnlyDictionary<int, string>> _grantedPermissionsFetched
            = LoggerMessage.Define<IReadOnlyDictionary<int, string>>(
                    LogLevel.Debug,
                    EventType.GrantedPermissionsFetched.ToEventId(),
                    "Granted Permissions fetched:\r\n\tGrantedPermissions: {GrantedPermissions}")
                .WithoutException();

        public static void GrantedPermissionsFetching(
                ILogger logger,
                ulong userId)
            => _grantedPermissionsFetching.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _grantedPermissionsFetching
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    EventType.GrantedPermissionsFetching.ToEventId(),
                    "Granted Permissions fetched: {UserId}")
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
                    EventType.GuildIdsFetched.ToEventId(),
                    "User Guild IDs fetched: UserId: {UserId}")
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
                    EventType.GuildIdsFetching.ToEventId(),
                    "Fetching User Guild IDs: UserId: {UserId}")
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
                    EventType.RoleUpdating.ToEventId(),
                    "Updating role: {RoleId}")
                .WithoutException();

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
                    EventType.UserIgnored.ToEventId(),
                    "User is neither admin or member: ignoring:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
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
                    EventType.UserInitializing.ToEventId(),
                    "Initializing user: {UserId}")
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
                    EventType.UserSignedIn.ToEventId(),
                    "User signed in:\r\n\tTicketId: {TicketId}\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
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
                    EventType.UserSigningIn.ToEventId(),
                    "User signing in:\r\n\tUserId: {UserId}\r\n\tUsername: {Username}\r\n\tDiscriminator: {Discriminator}")
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
                    EventType.UserTracked.ToEventId(),
                    "User tracked: {UserId}")
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
                    EventType.UserUpdating.ToEventId(),
                    "Updating user: {UserId}")
                .WithoutException();
    }
}
