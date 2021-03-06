﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace Sokan.Yastah.Api.Authentication
{
    [ServiceBinding(ServiceLifetime.Transient)]
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
    }
}
