using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Roles
{
    public interface IRoleRepository
    {
        Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateAsync(
            long roleId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted,
            CancellationToken cancellationToken);
    }

    public class RoleRepository
        : IRoleRepository
    {
        public RoleRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public async Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken)
        {
            var version = new RoleVersionEntity()
            {
                Role = new RoleEntity(),
                Name = name,
                IsDeleted = false,
                ActionId = actionId
            };

            await _context.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return version.Role.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long roleId,
            long actionId,
            Optional<string> name,
            Optional<bool> isDeleted,
            CancellationToken cancellationToken)
        {
            var currentVersion = await _context.Set<RoleVersionEntity>()
                .Where(x => x.RoleId == roleId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
                return new DataNotFoundError($"Role ID {roleId}")
                    .ToError<long>();

            var newVersion = new RoleVersionEntity()
            {
                RoleId = currentVersion.RoleId,
                Name = (name.IsSpecified)
                    ? name.Value
                    : currentVersion.Name,
                IsDeleted = (isDeleted.IsSpecified)
                    ? isDeleted.Value
                    : currentVersion.IsDeleted,
                ActionId = actionId,
                PreviousVersionId = currentVersion.Id
            };
            currentVersion.NextVersion = newVersion;

            await _context.AddAsync(newVersion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newVersion.Id
                .ToSuccess();
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IRoleRepository, RoleRepository>();
    }
}
