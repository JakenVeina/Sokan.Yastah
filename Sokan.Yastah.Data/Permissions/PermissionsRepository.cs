using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Permissions
{
    public interface IPermissionsRepository
    {
        IAsyncEnumerable<PermissionCategoryDescriptionViewModel> AsyncEnumerateDescriptions();

        IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateIdentities();

        IAsyncEnumerable<int> AsyncEnumeratePermissionIds(
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

        public IAsyncEnumerable<PermissionCategoryDescriptionViewModel> AsyncEnumerateDescriptions()
            => _context
                .Set<PermissionCategoryEntity>()
                .Select(PermissionCategoryDescriptionViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

        public IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateIdentities()
            => _context
                .Set<PermissionEntity>()
                .Select(PermissionIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

        public IAsyncEnumerable<int> AsyncEnumeratePermissionIds(
                Optional<IReadOnlyCollection<int>> permissionIds = default)
        {
            var query = _context.Set<PermissionEntity>()
                .AsQueryable();

            if (permissionIds.IsSpecified)
                query = query.Where(x => permissionIds.Value.Contains(x.PermissionId));

            return query
                .Select(x => x.PermissionId)
                .AsAsyncEnumerable();
        }

        private readonly YastahDbContext _context;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IPermissionsRepository, PermissionsRepository>();
    }
}
