using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public delegate void ConfigureServicesHandler(IServiceCollection services, IConfiguration configuration);
}
