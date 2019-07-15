using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Sokan.Yastah.Web
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            await webHost.Services
                .HandleStartupAsync();

            await webHost
                .RunAsync();
        }
    }
}
