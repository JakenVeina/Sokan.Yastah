using System;
using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using Sokan.Yastah.Business.Authentication;

namespace Sokan.Yastah.Api.Authentication
{
    internal static class AuthenticationLogMessages
    {
        public static void AuthenticationTokenNotFound(
                ILogger logger,
                string tokenHeaderAndPayloadCookieKey,
                string tokenSignatureCookieKey)
            => _authenticationTokenNotFound.Invoke(
                logger,
                tokenHeaderAndPayloadCookieKey,
                tokenSignatureCookieKey);
        private static readonly Action<ILogger, string, string> _authenticationTokenNotFound
            = LoggerMessage.Define<string, string>(
                    LogLevel.Warning,
                    new EventId(2001, nameof(AuthenticationTokenNotFound)),
                    "Authentication Token not found:\r\n\tTokenHeaderAndPayloadCookieKey: {TokenHeaderAndPayloadCookieKey}\r\n\tTokenSignatureCookieKey: {TokenSignatureCookieKey}")
                .WithoutException();

        public static void AuthenticationTicketNotIssued(
                ILogger logger,
                ulong userId)
            => _authenticationTicketNotIssued.Invoke(
                logger,
                userId);
        private static readonly Action<ILogger, ulong> _authenticationTicketNotIssued
            = LoggerMessage.Define<ulong>(
                    LogLevel.Warning,
                    new EventId(2002, nameof(AuthenticationTicketNotIssued)),
                    "AuthenticationTicket not issued:\r\n\tUserId: {UserId}")
                .WithoutException();

        public static void SignInHandling(
                ILogger logger)
            => _signInHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _signInHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4001, nameof(SignInHandling)),
                    "Handling Authentication SignIn event")
                .WithoutException();

        public static void SignInHandled(
                ILogger logger)
            => _signInHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _signInHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(SignInHandled)),
                    "Authentication SignIn handled")
                .WithoutException();

        public static void SignOutHandling(
                ILogger logger)
            => _signOutHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _signOutHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4003, nameof(SignOutHandling)),
                    "Handling Authentication SignOut")
                .WithoutException();

        public static void SignOutHandled(
                ILogger logger)
            => _signOutHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _signOutHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4004, nameof(SignOutHandled)),
                    "Authentication SignOut handled")
                .WithoutException();

        public static void AuthenticationPerforming(
                ILogger logger,
                long ticketId)
            => _authenticationPerforming.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationPerforming
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4005, nameof(AuthenticationPerforming)),
                    "Performing Authentication:\r\n\tTicketId: {TicketId}")
                .WithoutException();

        public static void AuthenticationPerformed(
                ILogger logger,
                AuthenticationTicket ticket)
            => _authenticationPerformed.Invoke(
                logger,
                ticket);
        private static readonly Action<ILogger, AuthenticationTicket> _authenticationPerformed
            = LoggerMessage.Define<AuthenticationTicket>(
                    LogLevel.Debug,
                    new EventId(4006, nameof(AuthenticationPerformed)),
                    "Authentication performed:\r\n\tTicket: {Ticket}")
                .WithoutException();

        public static void AuthenticationFailureHandling(
                ILogger logger)
            => _authenticationFailureHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationFailureHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4007, nameof(AuthenticationFailureHandling)),
                    "Handling Authentication Failure")
                .WithoutException();

        public static void AuthenticationFailureHandled(
                ILogger logger)
            => _authenticationFailureHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationFailureHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4008, nameof(AuthenticationFailureHandled)),
                    "Authentication Failure handled")
                .WithoutException();

        public static void AuthenticationTokenCreationHandling(
                ILogger logger)
            => _authenticationTokenCreationHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTokenCreationHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4009, nameof(AuthenticationTokenCreationHandling)),
                    "Handling Authentication Token creation")
                .WithoutException();

        public static void AuthenticationTokenCreationHandled(
                ILogger logger)
            => _authenticationTokenCreationHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTokenCreationHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4010, nameof(AuthenticationTokenCreationHandled)),
                    "Authentication Token creation handled")
                .WithoutException();

        public static void HttpMessageReceiptHandling(
                ILogger logger)
            => _httpMessageReceiptHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _httpMessageReceiptHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4011, nameof(HttpMessageReceiptHandling)),
                    "Handling receipt of HTTP Message")
                .WithoutException();

        public static void HttpMessageReceiptHandled(
                ILogger logger)
            => _httpMessageReceiptHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _httpMessageReceiptHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4012, nameof(HttpMessageReceiptHandled)),
                    "Receipt of HTTP Message handled")
                .WithoutException();

        public static void AuthenticationTokenValidationHandling(
                ILogger logger,
                SecurityToken token)
            => _authenticationTokenValidationHandling.Invoke(
                logger,
                token);
        private static readonly Action<ILogger, SecurityToken> _authenticationTokenValidationHandling
            = LoggerMessage.Define<SecurityToken>(
                    LogLevel.Debug,
                    new EventId(4013, nameof(AuthenticationTokenValidationHandling)),
                    "Handling Authentication Token validation:\r\n\tToken: {Token}")
                .WithoutException();

        public static void AuthenticationTokenValidationHandled(
                ILogger logger)
            => _authenticationTokenValidationHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTokenValidationHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4014, nameof(AuthenticationTokenValidationHandled)),
                    "Authentication Token validation handled")
                .WithoutException();

        public static void AuthenticationTicketCreationHandling(
                ILogger logger)
            => _authenticationTicketCreationHandling.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTicketCreationHandling
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4015, nameof(AuthenticationTicketCreationHandling)),
                    "Handling AuthenticationTicket creation")
                .WithoutException();

        public static void AuthenticationTicketCreationHandled(
                ILogger logger)
            => _authenticationTicketCreationHandled.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTicketCreationHandled
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4016, nameof(AuthenticationTicketCreationHandled)),
                    "AuthenticationTicket creation handled")
                .WithoutException();


        public static void AuthenticationTokenBuilding(
                ILogger logger,
                ClaimsIdentity identity,
                DateTime expires)
            => _authenticationTokenBuilding.Invoke(
                logger,
                identity,
                expires);
        private static readonly Action<ILogger, ClaimsIdentity, DateTime> _authenticationTokenBuilding
            = LoggerMessage.Define<ClaimsIdentity, DateTime>(
                    LogLevel.Debug,
                    new EventId(4017, nameof(AuthenticationTokenBuilding)),
                    "Building Authentication Token:\r\n\tIdentity: {Identity}\r\n\tExpires: {Expires}")
                .WithoutException();

        public static void AuthenticationTokenBuilt(
                ILogger logger,
                SecurityToken token)
            => _authenticationTokenBuilt.Invoke(
                logger,
                token);
        private static readonly Action<ILogger, SecurityToken> _authenticationTokenBuilt
            = LoggerMessage.Define<SecurityToken>(
                    LogLevel.Debug,
                    new EventId(4018, nameof(AuthenticationTokenBuilt)),
                    "Authentication Token built:\r\n\tToken: {Token}")
                .WithoutException();

        public static void AuthenticationTokenRenewing(
                ILogger logger)
            => _authenticationTokenRenewing.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTokenRenewing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4019, nameof(AuthenticationTokenRenewing)),
                    "Renewing Authentication Token")
                .WithoutException();

        public static void AuthenticationTokenRenewed(
                ILogger logger)
            => _authenticationTokenRenewed.Invoke(
                logger);
        private static readonly Action<ILogger> _authenticationTokenRenewed
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4020, nameof(AuthenticationTokenRenewed)),
                    "Authentication Token renewed")
                .WithoutException();

        public static void AuthenticationTicketIdClaimAdded(
                ILogger logger,
                long ticketId)
            => _authenticationTicketIdClaimAdded.Invoke(
                logger,
                ticketId);
        private static readonly Action<ILogger, long> _authenticationTicketIdClaimAdded
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4021, nameof(AuthenticationTicketIdClaimAdded)),
                    "AuthenticationTicket ID Claim added to AuthenticationToken:\r\n\tTicketId: {TicketId}")
                .WithoutException();

        public static void GrantedPermissionsClaimAdded(
                ILogger logger,
                IReadOnlyDictionary<int, string> grantedPermissions)
            => _grantedPermissionsClaimAdded.Invoke(
                logger,
                grantedPermissions);
        private static readonly Action<ILogger, IReadOnlyDictionary<int, string>> _grantedPermissionsClaimAdded
            = LoggerMessage.Define<IReadOnlyDictionary<int, string>>(
                    LogLevel.Debug,
                    new EventId(4022, nameof(GrantedPermissionsClaimAdded)),
                    "GrantedPermissions Claim added to AuthenticationToken:\r\n\tGrantedPermissions: {GrantedPermissions}")
                .WithoutException();

        public static void AuthenticationTokenHeaderAndPayloadAttached(
                ILogger logger,
                string cookieKey,
                string tokenHeaderAndPayload)
            => _authenticationTokenHeaderAndPayloadAttached.Invoke(
                logger,
                cookieKey,
                tokenHeaderAndPayload);
        private static readonly Action<ILogger, string, string> _authenticationTokenHeaderAndPayloadAttached
            = LoggerMessage.Define<string, string>(
                    LogLevel.Debug,
                    new EventId(4023, nameof(AuthenticationTokenHeaderAndPayloadAttached)),
                    "AuthenticationToken Header and Payload attached to HTTP Response:\r\n\tCookieKey: {CookieKey}\r\n\tTokenHeaderAndPayload: {TokenHeaderAndPayload}")
                .WithoutException();

        public static void AuthenticationTokenSignatureAttached(
                ILogger logger,
                string cookieKey,
                string tokenSignature)
            => _authenticationTokenSignatureAttached.Invoke(
                logger,
                cookieKey,
                tokenSignature);
        private static readonly Action<ILogger, string, string> _authenticationTokenSignatureAttached
            = LoggerMessage.Define<string, string>(
                    LogLevel.Debug,
                    new EventId(4024, nameof(AuthenticationTokenSignatureAttached)),
                    "AuthenticationToken Signature attached to HTTP Response:\r\n\tCookieKey: {CookieKey}\r\n\tTokenSignature: {TokenSignature}")
                .WithoutException();

        public static void AuthenticationTokenHeaderAndPayloadDetached(
                ILogger logger,
                string cookieKey)
            => _authenticationTokenHeaderAndPayloadDetached.Invoke(
                logger,
                cookieKey);
        private static readonly Action<ILogger, string> _authenticationTokenHeaderAndPayloadDetached
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(4025, nameof(AuthenticationTokenHeaderAndPayloadDetached)),
                    "AuthenticationToken Header and Payload detached to HTTP Response:\r\n\tCookieKey: {CookieKey}")
                .WithoutException();

        public static void AuthenticationTokenSignatureDetached(
                ILogger logger,
                string cookieKey)
            => _authenticationTokenSignatureDetached.Invoke(
                logger,
                cookieKey);
        private static readonly Action<ILogger, string> _authenticationTokenSignatureDetached
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(4026, nameof(AuthenticationTokenSignatureDetached)),
                    "AuthenticationToken Signature detached to HTTP Response:\r\n\tCookieKey: {CookieKey}")
                .WithoutException();

        public static void IssuingSignOutRedirect(
                ILogger logger,
                string redirectUri)
            => _issuingSignOutRedirect.Invoke(
                logger,
                redirectUri);
        private static readonly Action<ILogger, string> _issuingSignOutRedirect
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(4027, nameof(IssuingSignOutRedirect)),
                    "Issuing Redirect for Authentication SignOut: {RedirectUri}")
                .WithoutException();

        public static void AuthenticationTokenExtracted(
                ILogger logger,
                string token)
            => _authenticationTokenExtracted.Invoke(
                logger,
                token);
        private static readonly Action<ILogger, string> _authenticationTokenExtracted
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(4028, nameof(AuthenticationTokenExtracted)),
                    "AuthenticationToken extracted from HTTP Message: {Token}")
                .WithoutException();

        public static void AuthenticationTokenExpirationValidating(
                ILogger logger,
                DateTime validFrom)
            => _authenticationTokenExpirationValidating.Invoke(
                logger,
                validFrom);
        private static readonly Action<ILogger, DateTime> _authenticationTokenExpirationValidating
            = LoggerMessage.Define<DateTime>(
                    LogLevel.Debug,
                    new EventId(4028, nameof(AuthenticationTokenExpirationValidating)),
                    "Validating AuthenticationToken expiration:\r\n\tValidFrom: {ValidFrom}")
                .WithoutException();

        public static void AuthenticationTicketCreated(
                ILogger logger,
                AuthenticationTicket ticket)
            => _authenticationTicketCreated.Invoke(
                logger,
                ticket);
        private static readonly Action<ILogger, AuthenticationTicket> _authenticationTicketCreated
            = LoggerMessage.Define<AuthenticationTicket>(
                    LogLevel.Debug,
                    new EventId(4028, nameof(AuthenticationTicketCreated)),
                    "AuthenticationTicket created: {Ticket}")
                .WithoutException();

    }
}
