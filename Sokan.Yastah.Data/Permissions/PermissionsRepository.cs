using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class PermissionsRepository
        : IPermissionsRepository
    {
        public PermissionsRepository(
            YastahDbContext context,
            ILogger<PermissionsRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IAsyncEnumerable<PermissionCategoryDescriptionViewModel> AsyncEnumerateDescriptions()
        {
            using var logScope = _logger.BeginMemberScope();
            PermissionsLogMessages.CategoryDescriptionsEnumerating(_logger);

            var result = _context
                .Set<PermissionCategoryEntity>()
                .Select(PermissionCategoryDescriptionViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateIdentities()
        {
            using var logScope = _logger.BeginMemberScope();
            PermissionsLogMessages.PermissionIdentitiesEnumerating(_logger);

            var result = _context
                .Set<PermissionEntity>()
                .Select(PermissionIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<int> AsyncEnumeratePermissionIds(
                Optional<IReadOnlyCollection<int>> permissionIds = default)
        {
            using var logScope = _logger.BeginMemberScope();
            PermissionsLogMessages.PermissionIdsEnumerating(_logger, permissionIds);

            var query = _context.Set<PermissionEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (permissionIds.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(permissionIds));
                query = query.Where(x => permissionIds.Value.Contains(x.PermissionId));
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(x => x.PermissionId)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
    }
}
