using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sokan.Yastah.Common
{
    public static class CommonSetup
    {
        public static IServiceCollection AddYastahCommon(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddAssembly(Assembly.GetExecutingAssembly(), configuration);
    }
}
