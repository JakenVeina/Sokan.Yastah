using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Business
{
    public static class BusinessSetup
    {
        public static IServiceCollection AddYastahBusiness(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
    }
}
