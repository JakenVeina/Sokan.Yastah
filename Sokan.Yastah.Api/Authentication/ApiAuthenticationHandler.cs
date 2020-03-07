using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Sokan.Yastah.Api.Authentication
{
    public class ApiAuthenticationHandler
        : SignInAuthenticationHandler<ApiAuthenticationOptions>
    {
        #region Construction

        public ApiAuthenticationHandler(
                IOptionsMonitor<ApiAuthenticationOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder,
                ISystemClock clock,
                IApiAuthenticationTokenBuilder apiAuthenticationTokenBuilder)
            : base(options, logger, encoder, clock)
        {
            _apiAuthenticationTokenBuilder = apiAuthenticationTokenBuilder;
        }

        #endregion Construction

        #region AuthenticationHandler

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            => throw new NotSupportedException();

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            => throw new NotSupportedException();

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
            => throw new NotSupportedException();

        #endregion AuthenticationHandler
        
        #region SignInAuthenticationHandler

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            using var logScope = Logger.BeginMemberScope();
            AuthenticationLogMessages.SignInHandling(Logger);

            var token = _apiAuthenticationTokenBuilder.BuildToken(user.Identities.First());
            AuthenticationLogMessages.AuthenticationTokenBuilt(Logger, token);

            var tokenRawHeaderAndPayload = $"{token.RawHeader}.{token.RawPayload}";
            Response.Cookies.Append(
                key: Options.TokenHeaderAndPayloadCookieKey,
                value: tokenRawHeaderAndPayload,
                options: new CookieOptions()
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = false,
                    Secure = true,
                    Expires = token.ValidTo
                });
            AuthenticationLogMessages.AuthenticationTokenHeaderAndPayloadAttached(Logger, Options.TokenHeaderAndPayloadCookieKey, tokenRawHeaderAndPayload);

            Response.Cookies.Append(
                key: Options.TokenSignatureCookieKey,
                value: token.RawSignature,
                options: new CookieOptions()
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = true,
                    Secure = true,
                    Expires = token.ValidTo
                });
            AuthenticationLogMessages.AuthenticationTokenSignatureAttached(Logger, Options.TokenSignatureCookieKey, token.RawSignature);

            AuthenticationLogMessages.SignInHandled(Logger);
            return Task.CompletedTask;
        }

        #endregion SignInAuthenticationHandler

        #region SignOutAuthenticationHandler
        
        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            using var logScope = Logger.BeginMemberScope();
            AuthenticationLogMessages.SignOutHandling(Logger);

            Response.Cookies.Delete(Options.TokenHeaderAndPayloadCookieKey);
            AuthenticationLogMessages.AuthenticationTokenHeaderAndPayloadDetached(Logger, Options.TokenSignatureCookieKey);
            Response.Cookies.Delete(Options.TokenSignatureCookieKey);
            AuthenticationLogMessages.AuthenticationTokenSignatureDetached(Logger, Options.TokenSignatureCookieKey);

            if (properties.RedirectUri is string)
            {
                AuthenticationLogMessages.IssuingSignOutRedirect(Logger, properties.RedirectUri);
                Response.Redirect(properties.RedirectUri);
            }

            AuthenticationLogMessages.SignOutHandled(Logger);
            return Task.CompletedTask;
        }

        #endregion SignOutAuthenticationHandler

        #region State

        private readonly IApiAuthenticationTokenBuilder _apiAuthenticationTokenBuilder;

        #endregion State
    }
}
