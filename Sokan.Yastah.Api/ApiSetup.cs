using System.Reflection;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using AspNet.Security.OAuth.Discord;

using Sokan.Yastah.Api.Antiforgery;
using Sokan.Yastah.Api.Authentication;

namespace Sokan.Yastah.Api
{
    public static class ApiSetup
    {
        public static IServiceCollection AddYastahApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvcCore()
                .AddAuthorization()
                .AddFormatterMappings()
                .AddCors();

            services.AddAuthentication(ApiAuthenticationDefaults.AuthenticationScheme)
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

            return services
                .AddServices(Assembly.GetExecutingAssembly(), configuration)
                .AddAntiforgery();
        }

        public static IApplicationBuilder UseYastahApi(this IApplicationBuilder applicationBuilder)
            => applicationBuilder
                .UseRouting()
                .UseAuthentication()
                .UseMiddleware<AntiforgeryMiddleware>()
                .UseEndpoints(endpointRouteBuilder => endpointRouteBuilder
                    .MapControllers());
    }
}
