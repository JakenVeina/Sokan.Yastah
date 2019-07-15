using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AspNet.Security.OAuth.Discord;

namespace Sokan.Yastah.Api.Authentication
{
    public static class AuthenticationSetup
    {
        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(ApiAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(ApiAuthenticationDefaults.AuthenticationScheme, null)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = AuthenticationEventHandlers.OnMessageReceived
                    })
                .AddOAuth<DiscordAuthenticationOptions, DiscordAuthenticationHandler>(DiscordAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = AuthenticationEventHandlers.OnCreatingTicket
                    };

                    options.CallbackPath = "/api/authentication/signin";

                    options.ClaimActions.MapJsonKey("avtr", "avatar");
                    options.ClaimActions.MapJsonKey("dscm", "discriminator");

                    options.Scope.Add("guilds");
                });

            services
                .AddSingleton<IApiAuthenticationTokenBuilder, ApiAuthenticationTokenBuilder>();
        }
    }
}
