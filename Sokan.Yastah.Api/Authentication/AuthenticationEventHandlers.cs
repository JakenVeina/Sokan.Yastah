using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using Sokan.Yastah.Business.Authentication;

namespace Sokan.Yastah.Api.Authentication
{
    public static class AuthenticationEventHandlers
    {
        public static async Task OnCreatingTicket(OAuthCreatingTicketContext context)
        {
            var permissions = await context.HttpContext.RequestServices
                .GetRequiredService<IAuthenticationService>()
                .OnUserAuthenticatedAsync(
                    userId: ulong.Parse(context.Identity.Claims
                        .First(x => x.Type == ClaimTypes.NameIdentifier)
                        .Value),
                    username: context.Identity.Claims
                        .First(x => x.Type == ClaimTypes.Name)
                        .Value,
                    discriminator: context.Identity.Claims
                        .First(x => x.Type == "dscm")
                        .Value,
                    avatarHash: context.Identity.Claims
                        .First(x => x.Type == "avtr")
                        .Value,
                    getGuildIdsDelegate: () => GetGuildIds(context), 
                    context.HttpContext.RequestAborted);

            foreach (var permission in permissions)
                context.Identity.AddClaim(new Claim("prms", $"{permission.CategoryName}.{permission.PermissionName}"));
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

        private static async Task<IEnumerable<ulong>> GetGuildIds(OAuthCreatingTicketContext context)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint + "/guilds");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("An error occurred while retrieving user guild information.");
            }

            return JArray.Parse(await response.Content.ReadAsStringAsync())
                .Select(guild => ulong.Parse((guild as JObject)
                    .Property("id").Value.ToString()));
        }
    }
}
