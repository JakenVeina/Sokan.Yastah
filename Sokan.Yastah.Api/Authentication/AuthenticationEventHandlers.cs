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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

using IAuthenticationService = Sokan.Yastah.Business.Authentication.IAuthenticationService;

namespace Sokan.Yastah.Api.Authentication
{
    public static class AuthenticationEventHandlers
    {
        #region Publis Methods

        public static Task OnAuthenticationFailed(AuthenticationFailedContext context)
        {
            var logger = GetLogger(context.HttpContext.RequestServices);
            AuthenticationLogMessages.AuthenticationFailureHandling(logger);

            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;

            context.Response.Cookies.Delete(options.TokenHeaderAndPayloadCookieKey);
            AuthenticationLogMessages.AuthenticationTokenHeaderAndPayloadDetached(logger, options.TokenSignatureCookieKey);
            context.Response.Cookies.Delete(options.TokenSignatureCookieKey);
            AuthenticationLogMessages.AuthenticationTokenSignatureDetached(logger, options.TokenSignatureCookieKey);

            AuthenticationLogMessages.AuthenticationFailureHandled(logger);
            return Task.CompletedTask;
        }

        public static async Task OnCreatingTicket(OAuthCreatingTicketContext context)
        {
            var logger = GetLogger(context.HttpContext.RequestServices);
            AuthenticationLogMessages.AuthenticationTicketCreationHandling(logger);

            var userId = ulong.Parse(context.Identity.Claims
                .First(x => x.Type == ClaimTypes.NameIdentifier)
                .Value);

            var ticket = await context.HttpContext.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .OnSignInAsync(
                    userId: userId,
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
            {
                AuthenticationLogMessages.AuthenticationTicketNotIssued(logger, userId);
                return;
            }
            AuthenticationLogMessages.AuthenticationTicketCreated(logger, ticket);

            context.Identity.AddClaim(new Claim(
                ApiAuthenticationDefaults.TicketIdClaimType,
                ticket.Id.ToString(),
                ClaimValueTypes.Integer64));
            AuthenticationLogMessages.AuthenticationTicketIdClaimAdded(logger, ticket.Id);

            context.Identity.AddClaim(new Claim(
                ApiAuthenticationDefaults.PermissionsClaimType,
                JsonConvert.SerializeObject(ticket.GrantedPermissions, _jsonSerializerSettings),
                JsonClaimValueTypes.Json));
            AuthenticationLogMessages.GrantedPermissionsClaimAdded(logger, ticket.GrantedPermissions);

            AuthenticationLogMessages.AuthenticationTicketCreationHandled(logger);
        }

        public static Task OnMessageReceived(MessageReceivedContext context)
        {
            var logger = GetLogger(context.HttpContext.RequestServices);
            AuthenticationLogMessages.HttpMessageReceiptHandling(logger);

            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;
            var cookies = context.Request.Cookies;

            if(cookies.TryGetValue(options.TokenHeaderAndPayloadCookieKey, out var tokenHeaderAndPayload)
                    && cookies.TryGetValue(options.TokenSignatureCookieKey, out var tokenSignature))
            {
                context.Token = $"{tokenHeaderAndPayload}.{tokenSignature}";
                AuthenticationLogMessages.AuthenticationTokenExtracted(logger, context.Token);
            }
            AuthenticationLogMessages.AuthenticationTokenNotFound(logger, options.TokenHeaderAndPayloadCookieKey, options.TokenSignatureCookieKey);

            AuthenticationLogMessages.HttpMessageReceiptHandled(logger);
            return Task.CompletedTask;
        }

        public static async Task OnTokenValidated(TokenValidatedContext context)
        {
            var logger = GetLogger(context.HttpContext.RequestServices);
            AuthenticationLogMessages.AuthenticationTokenValidationHandling(logger, context.SecurityToken);

            var jwtSecurityToken = (JwtSecurityToken)context.SecurityToken;

            var ticketId = (long)jwtSecurityToken.Payload[ApiAuthenticationDefaults.TicketIdClaimType];

            AuthenticationLogMessages.AuthenticationPerforming(logger, ticketId);
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
            AuthenticationLogMessages.AuthenticationPerformed(logger, ticket);

            var renewSignIn = ticket.Id != ticketId;

            if(!renewSignIn)
            {
                AuthenticationLogMessages.AuthenticationTokenExpirationValidating(logger, jwtSecurityToken.ValidFrom);
                
                var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiAuthenticationOptions>>().Value;
                var now = context.HttpContext.RequestServices.GetRequiredService<ISystemClock>().UtcNow;

                renewSignIn = (now - jwtSecurityToken.ValidFrom) > options.TokenRefreshInterval;
            }

            if (renewSignIn)
            {
                AuthenticationLogMessages.AuthenticationTokenRenewing(logger);

                var identity = context.Principal.Identities.First();
                identity.RemoveClaim(identity.FindFirst(ApiAuthenticationDefaults.TicketIdClaimType));
                identity.AddClaim(new Claim(
                    ApiAuthenticationDefaults.TicketIdClaimType,
                    ticket.Id.ToString(),
                    ClaimValueTypes.Integer64));

                await context.HttpContext.SignInAsync(ApiAuthenticationDefaults.AuthenticationScheme, context.Principal);
                AuthenticationLogMessages.AuthenticationTokenRenewed(logger);
            }

            AuthenticationLogMessages.AuthenticationTokenValidationHandled(logger);
        }

        #endregion Publis Methods

        #region Private Methods

        private static ILogger GetLogger(
                IServiceProvider serviceProvider)
            => serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(nameof(AuthenticationEventHandlers));

        private static async Task<IEnumerable<ulong>> GetGuildIds(
            OAuthCreatingTicketContext context,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint + "/guilds");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            using var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("An error occurred while retrieving user guild information.");
            }

            return JArray.Parse(await response.Content.ReadAsStringAsync())
                .Select(guild => ulong.Parse(((JObject)guild)
                    .Property("id").Value.ToString()));
        }

        #endregion Private Methods

        #region State

        private static readonly JsonSerializerSettings _jsonSerializerSettings
            = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        #endregion State
    }
}
