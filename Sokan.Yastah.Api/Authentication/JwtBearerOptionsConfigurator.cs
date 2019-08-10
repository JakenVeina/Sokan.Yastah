using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace Sokan.Yastah.Api.Authentication
{
    public class JwtBearerOptionsConfigurator
        : IPostConfigureOptions<JwtBearerOptions>
    {
        public JwtBearerOptionsConfigurator(
            IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            _authenticationConfiguration = authenticationConfiguration.Value;
        }

        public void PostConfigure(string name, JwtBearerOptions options)
            => options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_authenticationConfiguration.TokenSecret));

        private readonly AuthenticationConfiguration _authenticationConfiguration;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddTransient<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>();
    }
}
