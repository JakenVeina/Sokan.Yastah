using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Host
{
    public static class Program
    {
        public static async Task Main()
        {
            var webHost = new WebHostBuilder()
                .UseSetting(WebHostDefaults.ApplicationKey, "Sokan.Yastah")
                .UseKestrel((context, options) =>
                {
                    options.Configure(context.Configuration.GetSection("Kestrel"));
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();

                    if (context.HostingEnvironment.IsDevelopment())
                        configurationBuilder
                            .AddUserSecrets<Startup>(optional: true);
                })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder
                        .AddConfiguration(context.Configuration.GetSection("Logging"))
                        .AddConsole();

                    if (context.HostingEnvironment.IsDevelopment())
                        loggingBuilder
                            .AddDebug();
                })
                .Build();

            await webHost.Services
                .HandleStartupAsync(CancellationToken.None);

            await webHost
                .RunAsync();
        }
    }
}
