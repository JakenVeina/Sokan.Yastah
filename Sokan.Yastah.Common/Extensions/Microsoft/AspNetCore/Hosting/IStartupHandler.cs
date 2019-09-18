using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Hosting
{
    public interface IStartupHandler
    {
        Task OnStartupAsync(CancellationToken cancellationToken);
    }
}
