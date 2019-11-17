using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using AspNet.Security.OAuth.Discord;

namespace Sokan.Yastah.Api.Authentication
{
    public static class AuthenticationSetup
    {
        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(ApiAuthenticationDefaults.AuthenticationScheme)
                .AddScheme<ApiAuthenticationOptions, ApiAuthenticationHandler>(ApiAuthenticationDefaults.AuthenticationScheme, null)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = false,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidateTokenReplay = false
                    };

                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = AuthenticationEventHandlers.OnAuthenticationFailed,
                        OnTokenValidated = AuthenticationEventHandlers.OnTokenValidated,
                        OnMessageReceived = AuthenticationEventHandlers.OnMessageReceived
                    };
                })
                .AddOAuth<DiscordAuthenticationOptions, DiscordAuthenticationHandler>(DiscordAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = AuthenticationEventHandlers.OnCreatingTicket
                    };

                    options.CallbackPath = "/api/authentication/signin";

                    options.ClaimActions.MapJsonKey(ApiAuthenticationDefaults.AvatarHashClaimType, "avatar");
                    options.ClaimActions.MapJsonKey(ApiAuthenticationDefaults.DiscriminatorClaimType, "discriminator");

                    options.Scope.Add("guilds");
                });

            services
                .AddSingleton<IApiAuthenticationTokenBuilder, ApiAuthenticationTokenBuilder>();
        }
    }
}
