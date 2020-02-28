using System;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Authentication
{
    public static class AuthenticationLogMessages
    {
        public static void AuthenticationTicketNotFound(
                ILogger logger,
                long ticketId)
            => _authenticationTicketNotFound(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(AuthenticationTicketNotFound)),
                    $"{nameof(AuthenticationTicketEntity)} not found: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketAlreadyDeleted(
                ILogger logger,
                long ticketId)
            => _authenticationTicketAlreadyDeleted(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketAlreadyDeleted
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    new EventId(2002, nameof(AuthenticationTicketAlreadyDeleted)),
                    $"{nameof(AuthenticationTicketEntity)} already deleted: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketNotFoundForUser(
                ILogger logger,
                ulong userId)
            => _authenticationTicketNotFoundForUser(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _authenticationTicketNotFoundForUser
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    new EventId(2003, nameof(AuthenticationTicketNotFoundForUser)),
                    $"{nameof(AuthenticationTicketEntity)} not found for user: {{UserId}}")
                .WithoutException();

        public static void AuthenticationTicketCreating(
                ILogger logger,
                ulong userId,
                long actionId)
            => _authenticationTicketCreating.Invoke(
                logger,
                userId,
                actionId);
        private static readonly Action<ILogger, ulong, long> _authenticationTicketCreating
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Information,
                    new EventId(3001, nameof(AuthenticationTicketCreating)),
                    $"Creating {nameof(AuthenticationTicketEntity)}: \r\n\tUserId: {{UserId}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void AuthenticationTicketCreated(
                ILogger logger,
                long ticketId)
            => _authenticationTicketCreated.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3002, nameof(AuthenticationTicketCreated)),
                    $"{nameof(AuthenticationTicketEntity)} created: {{TicketId}}")
                .WithoutException();

        public static void AuthenticationTicketDeleting(
                ILogger logger,
                long ticketId,
                long actionId)
            => _authenticationTicketDeleting.Invoke(
                logger,
                ticketId,
                actionId);
        private static readonly Action<ILogger, long, long> _authenticationTicketDeleting
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    new EventId(3003, nameof(AuthenticationTicketDeleting)),
                    $"Deleting {nameof(AuthenticationTicketEntity)}: \r\n\tTicketId: {{TicketId}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void AuthenticationTicketDeleted(
                ILogger logger,
                long ticketId)
            => _authenticationTicketAlreadyDeleted.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketDeleted
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    new EventId(3004, nameof(AuthenticationTicketDeleted)),
                    $"{nameof(AuthenticationTicketEntity)} deleted: {{TicketId}}")
                .WithoutException();

        public static void IdentitiesEnumerating(
                ILogger logger,
                Optional<bool> isDeleted)
            => _identitiesEnumerating.Invoke(
                logger,
                isDeleted);
        private static readonly Action<ILogger, Optional<bool>> _identitiesEnumerating
            = LoggerMessage.Define<Optional<bool>>(
                    LogLevel.Debug,
                    new EventId(4001, nameof(IdentitiesEnumerating)),
                    $"Enumerating {nameof(AuthenticationTicketIdentityViewModel)}: \r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void ActiveIdReading(
                ILogger logger,
                ulong userId)
            => _activeIdReading.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _activeIdReading
            = LoggerMessage.Define<ulong>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(ActiveIdReading)),
                    $"Reading {nameof(AuthenticationTicketEntity)} Active Id: \r\n\tUserId: {{UserId}}")
                .WithoutException();

        public static void ActiveIdRead(
                ILogger logger,
                ulong userId,
                long ticketId)
            => _activeIdRead.Invoke(
                logger,
                userId,
                ticketId);
        private static readonly Action<ILogger, ulong, long> _activeIdRead
            = LoggerMessage.Define<ulong, long>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(ActiveIdRead)),
                    $"Active {nameof(AuthenticationTicketEntity)} found: \r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
                .WithoutException();
    }
}
