using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Permissions
{
    public interface IPermissionsRepository
    {
        Task<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> ReadDescriptionsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PermissionIdentityViewModel>> ReadIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<int>> ReadPermissionIdsAsync(
            CancellationToken cancellationToken,
            Optional<IReadOnlyCollection<int>> permissionIds = default);
    }

    public class PermissionsRepository
        : IPermissionsRepository
    {
        public PermissionsRepository(
            YastahDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> ReadDescriptionsAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<PermissionCategoryEntity>()
                .Select(PermissionCategoryDescriptionViewModel.FromEntityProjection)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyCollection<PermissionIdentityViewModel>> ReadIdentitiesAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<PermissionEntity>()
                .Select(PermissionIdentityViewModel.FromEntityProjection)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyCollection<int>> ReadPermissionIdsAsync(
                CancellationToken cancellationToken,
                Optional<IReadOnlyCollection<int>> permissionIds = default)
        {
            var query = _context.Set<PermissionEntity>()
                .AsQueryable();

            if (permissionIds.IsSpecified)
                query = query.Where(x => permissionIds.Value.Contains(x.PermissionId));

            return await query
                .Select(x => x.PermissionId)
                .ToArrayAsync(cancellationToken);
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IPermissionsRepository, PermissionsRepository>();
    }
}
