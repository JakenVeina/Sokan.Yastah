using System.Reflection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data
{
    public static class DataSetup
    {
        public static IServiceCollection AddYastahData(this IServiceCollection services, IConfiguration configuration)
            => services.AddServices(Assembly.GetExecutingAssembly(), configuration);
    }
}
