using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Business
{
    public static class BusinessSetup
    {
        public static IServiceCollection AddYastahBusiness(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddMemoryCache()
                .AddServices(Assembly.GetExecutingAssembly(), configuration);
    }
}
