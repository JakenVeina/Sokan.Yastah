using System.Reflection;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Sokan.Yastah.Common
{
    public static class CommonSetup
    {
        public static IServiceCollection AddYastahCommon(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
    }
}
