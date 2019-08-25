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
            = "Yastah.Api.Authentication.Ticket.HeaderAndPayload";

        public const string TokenSignatureCookieKey
            = "Yastah.Api.Authentication.Ticket.Signature";

        public static readonly TimeSpan TokenLifetime
            = TimeSpan.FromDays(2);

        public static readonly TimeSpan TokenRefreshInterval
            = TimeSpan.FromDays(1);

        public const string TicketIdClaimType
            = "tckt";

        public const string DiscriminatorClaimType
            = "dscm";

        public const string AvatarHashClaimType
            = "avtr";

        public const string PermissionsClaimType
            = "prms";
    }
}
