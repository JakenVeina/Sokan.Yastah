using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            IOptions<ApiAuthenticationOptions> options)
        {
            _tokenHandler = new JwtSecurityTokenHandler();
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        options.Value.TokenSecret)),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public JwtSecurityToken BuildToken(ClaimsIdentity identity)
            => _tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor()
            {
                Subject = identity,
                SigningCredentials = _signingCredentials
            });

        private readonly JwtSecurityTokenHandler _tokenHandler;

        private readonly SigningCredentials _signingCredentials;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddSingleton<IApiAuthenticationTokenBuilder, ApiAuthenticationTokenBuilder>();
    }
}
