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

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            => throw new NotSupportedException();

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
            => throw new NotSupportedException();

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
            => throw new NotSupportedException();

        protected override Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var token = _apiAuthenticationTokenBuilder.BuildToken(user.Identities.First());

            Response.Cookies.Append(
                key: Options.TokenHeaderAndPayloadCookieKey,
                value: $"{token.RawHeader}.{token.RawPayload}",
                options: new CookieOptions()
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Strict,
                    HttpOnly = false,
                    Secure = true,
                    Expires = token.ValidTo
                });

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

            return Task.CompletedTask;
        }

        protected override Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            Response.Cookies.Delete(Options.TokenHeaderAndPayloadCookieKey);
            Response.Cookies.Delete(Options.TokenSignatureCookieKey);

            if(properties.RedirectUri is string)
                Response.Redirect(properties.RedirectUri);

            return Task.CompletedTask;
        }

        private readonly IApiAuthenticationTokenBuilder _apiAuthenticationTokenBuilder;
    }
}
