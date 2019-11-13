using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Host
{
    public static class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
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
                .ConfigureWebHost(webHostBuilder =>
                {
                    webHostBuilder
                        .UseSetting(WebHostDefaults.ApplicationKey, "Sokan.Yastah")
                        .UseKestrel((context, options) =>
                        {
                            options.Configure(context.Configuration.GetSection("Kestrel"));
                        })
                        .UseStartup<Startup>();
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

            await host.Services
                .HandleStartupAsync(CancellationToken.None);

            await host
                .RunAsync();
        }
    }
}
