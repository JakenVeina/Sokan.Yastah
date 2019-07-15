using System;

namespace Sokan.Yastah.Api.Authentication
{
    public static class ApiAuthenticationDefaults
    {
        public const string AuthenticationScheme
            = "Yastah.Api";

        public const string Issuer
            = "Yastah.Api";

        public const string TokenHeaderAndPayloadCookieKey
            = "Yastah.Api.Token.HeaderAndPayload";

        public const string TokenSignatureCookieKey
            = "Yastah.Api.Token.Signature";

        public static readonly TimeSpan TokenLifetime
            = TimeSpan.FromDays(1);
    }
}
