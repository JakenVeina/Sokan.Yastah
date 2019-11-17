using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Sokan.Yastah.Api.Authentication
{
    public interface IApiAuthenticationTokenBuilder
    {
        JwtSecurityToken BuildToken(ClaimsIdentity identity);
    }

    public class ApiAuthenticationTokenBuilder
        : IApiAuthenticationTokenBuilder
    {
        public ApiAuthenticationTokenBuilder(
            IOptions<ApiAuthenticationOptions> options,
            ISystemClock systemClock)
        {
            _options = options;
            _systemClock = systemClock;

            _tokenHandler = new JwtSecurityTokenHandler();
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        options.Value.TokenSecret)),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public JwtSecurityToken BuildToken(ClaimsIdentity identity)
        {
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = identity,
                SigningCredentials = _signingCredentials,
                Expires = _systemClock.UtcNow.UtcDateTime + _options.Value.TokenLifetime
            };

            return _tokenHandler.CreateJwtSecurityToken(descriptor);
        }

        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IOptions<ApiAuthenticationOptions> _options;
        private readonly SigningCredentials _signingCredentials;
        private readonly ISystemClock _systemClock;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddSingleton<IApiAuthenticationTokenBuilder, ApiAuthenticationTokenBuilder>();
    }
}
