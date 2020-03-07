using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Sokan.Yastah.Api.Authentication
{
    public interface IApiAuthenticationTokenBuilder
    {
        JwtSecurityToken BuildToken(ClaimsIdentity identity);
    }

    [ServiceBinding(ServiceLifetime.Singleton)]
    public class ApiAuthenticationTokenBuilder
        : IApiAuthenticationTokenBuilder
    {
        #region Construction

        public ApiAuthenticationTokenBuilder(
            ILogger<ApiAuthenticationTokenBuilder> logger,
            IOptions<ApiAuthenticationOptions> options,
            ISystemClock systemClock)
        {
            _logger = logger;
            _options = options;
            _systemClock = systemClock;

            _tokenHandler = new JwtSecurityTokenHandler();
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(
                        options.Value.TokenSecret)),
                SecurityAlgorithms.HmacSha256Signature);
        }

        #endregion Construction

        #region IApiAuthenticationTokenBuilder

        public JwtSecurityToken BuildToken(ClaimsIdentity identity)
        {
            var expires = _systemClock.UtcNow.UtcDateTime + _options.Value.TokenLifetime;

            AuthenticationLogMessages.AuthenticationTokenBuilding(_logger, identity, expires);
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = identity,
                SigningCredentials = _signingCredentials,
                Expires = expires
            };

            var token = _tokenHandler.CreateJwtSecurityToken(descriptor);
            AuthenticationLogMessages.AuthenticationTokenBuilt(_logger, token);

            return token;
        }

        #endregion IApiAuthenticationTokenBuilder

        #region State

        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly ILogger _logger;
        private readonly IOptions<ApiAuthenticationOptions> _options;
        private readonly SigningCredentials _signingCredentials;
        private readonly ISystemClock _systemClock;

        #endregion State
    }
}
