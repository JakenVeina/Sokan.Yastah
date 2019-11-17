using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public delegate void ConfigureServicesWithConfigurationHandler(IServiceCollection services, IConfiguration configuration);
}
