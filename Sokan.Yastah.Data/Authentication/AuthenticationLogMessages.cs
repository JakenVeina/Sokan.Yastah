using System;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Authentication
{
    internal static class AuthenticationLogMessages
    {
        public enum EventType
        {
            IdentitiesEnumerating               = DataLogEventType.Authentication + 0x0001,
            AuthenticationTicketCreating        = DataLogEventType.Authentication + 0x0002,
            AuthenticationTicketCreated         = DataLogEventType.Authentication + 0x0003,
            AuthenticationTicketDeleting        = DataLogEventType.Authentication + 0x0004,
            AuthenticationTicketDeleted         = DataLogEventType.Authentication + 0x0005,
            AuthenticationTicketAlreadyDeleted  = DataLogEventType.Authentication + 0x0006,
            ActiveIdReading                     = DataLogEventType.Authentication + 0x0007,
            ActiveIdRead                        = DataLogEventType.Authentication + 0x0008,
            AuthenticationTicketNotFound        = DataLogEventType.Authentication + 0x0009,
            AuthenticationTicketNotFoundForUser = DataLogEventType.Authentication + 0x000A
        }

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
                    EventType.ActiveIdRead.ToEventId(),
                    $"Active {nameof(AuthenticationTicketEntity)} found: \r\n\tUserId: {{UserId}}\r\n\tTicketId: {{TicketId}}")
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
                    EventType.ActiveIdReading.ToEventId(),
                    $"Reading {nameof(AuthenticationTicketEntity)} Active Id: \r\n\tUserId: {{UserId}}")
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
                    EventType.AuthenticationTicketAlreadyDeleted.ToEventId(),
                    $"{nameof(AuthenticationTicketEntity)} already deleted: {{TicketId}}")
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
                    EventType.AuthenticationTicketCreated.ToEventId(),
                    $"{nameof(AuthenticationTicketEntity)} created: {{TicketId}}")
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
                    EventType.AuthenticationTicketCreating.ToEventId(),
                    $"Creating {nameof(AuthenticationTicketEntity)}: \r\n\tUserId: {{UserId}}\r\n\tActionId: {{ActionId}}")
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
                    EventType.AuthenticationTicketDeleted.ToEventId(),
                    $"{nameof(AuthenticationTicketEntity)} deleted: {{TicketId}}")
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
                    EventType.AuthenticationTicketDeleting.ToEventId(),
                    $"Deleting {nameof(AuthenticationTicketEntity)}: \r\n\tTicketId: {{TicketId}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void AuthenticationTicketNotFound(
                ILogger logger,
                long ticketId)
            => _authenticationTicketNotFound(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Warning,
                    EventType.AuthenticationTicketNotFound.ToEventId(),
                    $"{nameof(AuthenticationTicketEntity)} not found: {{TicketId}}")
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
                    EventType.AuthenticationTicketNotFoundForUser.ToEventId(),
                    $"{nameof(AuthenticationTicketEntity)} not found for user: {{UserId}}")
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
                    EventType.IdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(AuthenticationTicketIdentityViewModel)}: \r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();
    }
}
