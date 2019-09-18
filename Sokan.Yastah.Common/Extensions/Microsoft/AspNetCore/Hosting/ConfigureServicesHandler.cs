using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Hosting
{
    public delegate void ConfigureServicesHandler(IServiceCollection services, IConfiguration configuration);
}
