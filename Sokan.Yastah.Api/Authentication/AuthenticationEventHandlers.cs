using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using IAuthenticationService = Sokan.Yastah.Business.Authentication.IAuthenticationService;

namespace Sokan.Yastah.Api.Authentication
{
    public static class AuthenticationEventHandlers
    {
        public static Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;

            context.Response.Cookies.Delete(options.TokenHeaderAndPayloadCookieKey);
            context.Response.Cookies.Delete(options.TokenSignatureCookieKey);

            return Task.CompletedTask;
        }

        public static async Task OnCreatingTicket(OAuthCreatingTicketContext context)
        {
            var ticket = await context.HttpContext.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .OnSignInAsync(
                    userId: ulong.Parse(context.Identity.Claims
                        .First(x => x.Type == ClaimTypes.NameIdentifier)
                        .Value),
                    username: context.Identity.Claims
                        .First(x => x.Type == ClaimTypes.Name)
                        .Value,
                    discriminator: context.Identity.Claims
                        .First(x => x.Type == ApiAuthenticationDefaults.DiscriminatorClaimType)
                        .Value,
                    avatarHash: context.Identity.Claims
                        .First(x => x.Type == ApiAuthenticationDefaults.AvatarHashClaimType)
                        .Value,
                    getGuildIdsDelegate: cancellationToken => GetGuildIds(context, cancellationToken), 
                    context.HttpContext.RequestAborted);

            if (ticket is null)
                return;

            context.Identity.AddClaim(new Claim(
                ApiAuthenticationDefaults.TicketIdClaimType,
                ticket.Id.ToString(),
                ClaimValueTypes.Integer64));

            context.Identity.AddClaim(new Claim(
                ApiAuthenticationDefaults.PermissionsClaimType,
                JsonConvert.SerializeObject(ticket.GrantedPermissions, _jsonSerializerSettings),
                JsonClaimValueTypes.Json));
        }

        public static Task OnMessageReceived(MessageReceivedContext context)
        {
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;
            var cookies = context.Request.Cookies;

            if(cookies.TryGetValue(options.TokenHeaderAndPayloadCookieKey, out var tokenHeaderAndPayload)
                    && cookies.TryGetValue(options.TokenSignatureCookieKey, out var tokenSignature))
                context.Token = $"{tokenHeaderAndPayload}.{tokenSignature}";

            return Task.CompletedTask;
        }

        public static async Task OnTokenValidated(TokenValidatedContext context)
        {
            var jwtSecurityToken = (JwtSecurityToken)context.SecurityToken;

            var ticketId = (long)jwtSecurityToken.Payload[ApiAuthenticationDefaults.TicketIdClaimType];

            var ticket = await context.HttpContext.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .OnAuthenticatedAsync(
                    ticketId: ticketId,
                    userId: ((string)jwtSecurityToken.Payload["nameid"])
                        .ParseUInt64(),
                    username: (string)jwtSecurityToken.Payload["unique_name"],
                    discriminator: (string)jwtSecurityToken.Payload[ApiAuthenticationDefaults.DiscriminatorClaimType],
                    avatarHash: (string)jwtSecurityToken.Payload[ApiAuthenticationDefaults.AvatarHashClaimType],
                    grantedPermissions: ((JObject)jwtSecurityToken.Payload[ApiAuthenticationDefaults.PermissionsClaimType])
                        .ToObject<Dictionary<int, string>>(),
                    context.HttpContext.RequestAborted);

            var renewSignIn = ticket.Id != ticketId;

            if(!renewSignIn)
            {
                var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;
                var now = context.HttpContext.RequestServices.GetRequiredService<ISystemClock>().UtcNow;

                renewSignIn = (now - jwtSecurityToken.ValidFrom) > options.TokenRefreshInterval;
            }

            if (renewSignIn)
            {
                var identity = context.Principal.Identities.First();
                identity.RemoveClaim(identity.FindFirst(ApiAuthenticationDefaults.TicketIdClaimType));
                identity.AddClaim(new Claim(
                    ApiAuthenticationDefaults.TicketIdClaimType,
                    ticket.Id.ToString(),
                    ClaimValueTypes.Integer64));

                await context.HttpContext.SignInAsync(ApiAuthenticationDefaults.AuthenticationScheme, context.Principal);
            }
        }

        private static async Task<IEnumerable<ulong>> GetGuildIds(
            OAuthCreatingTicketContext context,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint + "/guilds");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("An error occurred while retrieving user guild information.");
            }

            return JArray.Parse(await response.Content.ReadAsStringAsync())
                .Select(guild => ulong.Parse((guild as JObject)
                    .Property("id").Value.ToString()));
        }

        private static JsonSerializerSettings _jsonSerializerSettings
            = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
    }
}
