using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public interface IStartupHandler
    {
        Task OnStartupAsync(CancellationToken cancellationToken);
    }
}
