using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Data.Permissions
{
    public interface IPermissionRepository
    {
        Task<IReadOnlyCollection<PermissionIdentity>> GetAllPermissionIdentitiesAsync(
            CancellationToken cancellationToken);
    }

    public class PermissionRepository
        : IPermissionRepository
    {
        public PermissionRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<PermissionIdentity>> GetAllPermissionIdentitiesAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<PermissionEntity>()
                .AsNoTracking()
                .Select(PermissionIdentity.FromEntityProjection)
                .ToArrayAsync(cancellationToken);

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IPermissionRepository, PermissionRepository>();
    }
}
