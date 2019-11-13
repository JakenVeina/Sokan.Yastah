using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Api;
using Sokan.Yastah.Business;
using Sokan.Yastah.Common;
using Sokan.Yastah.Data;
using Sokan.Yastah.Web;

namespace Sokan.Yastah.Host
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
            => services
                .AddSingleton<ISystemClock, SystemClock>()
                .AddYastahCommon(_configuration)
                .AddYastahData(_configuration)
                .AddYastahBusiness(_configuration)
                .AddYastahApi(_configuration)
                .AddYastahWeb(_configuration);

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            if (!_webHostEnvironment.IsDevelopment())
                applicationBuilder
                    .UseHsts();

            applicationBuilder
                .UseHttpsRedirection()
                .MapWhen(IsApiPath, apiBuilder => apiBuilder
                   .UseYastahApi())
                .MapWhen(IsWebPath, webBuilder => webBuilder
                   .UseYastahWeb());
        }

        private static bool IsApiPath(HttpContext context)
            => context.Request.Path.Value.ToLower().StartsWith("/api");

        private static bool IsWebPath(HttpContext context)
            => !IsApiPath(context);

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
    }
}
