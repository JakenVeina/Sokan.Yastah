using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using AspNet.Security.OAuth.Discord;

namespace Sokan.Yastah.Api.Authentication
{
    public class ApiAuthenticationOptions
        : AuthenticationSchemeOptions
    {
        public ApiAuthenticationOptions()
        {
            ClaimsIssuer = ApiAuthenticationDefaults.Issuer;
            ForwardAuthenticate = JwtBearerDefaults.AuthenticationScheme;
            ForwardChallenge = DiscordAuthenticationDefaults.AuthenticationScheme;
            ForwardForbid = JwtBearerDefaults.AuthenticationScheme;
            TokenHeaderAndPayloadCookieKey = ApiAuthenticationDefaults.TokenHeaderAndPayloadCookieKey;
            TokenLifetime = ApiAuthenticationDefaults.TokenLifetime;
            TokenSignatureCookieKey = ApiAuthenticationDefaults.TokenSignatureCookieKey;
        }

        public string TokenHeaderAndPayloadCookieKey { get; set; }

        public TimeSpan TokenLifetime { get; set; }

        public string TokenSecret { get; set; }

        public string TokenSignatureCookieKey { get; set; }
    }
}
