using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Roles
{
    public interface IRolesRepository
    {
        Task<bool> AnyVersionsAsync(
            CancellationToken cancellationToken,
            Optional<IEnumerable<long>> excludedRoleIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default);

        IAsyncEnumerable<RoleIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default);

        IAsyncEnumerable<RolePermissionMappingIdentity> AsyncEnumeratePermissionMappingIdentities(
            long roleId,
            Optional<bool> isDeleted = default);

        Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<long>> CreatePermissionMappingsAsync(
            long roleId,
            IEnumerable<int> permissionIds,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<RoleDetailViewModel>> ReadDetailAsync(
            CancellationToken cancellationToken,
            long roleId,
            Optional<bool> isDeleted = default);

        Task<OperationResult<long>> UpdateAsync(
            CancellationToken cancellationToken,
            long roleId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default);

        Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken);
    }

    public class RolesRepository
        : IRolesRepository
    {
        public RolesRepository(
            YastahDbContext context,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyVersionsAsync(
            CancellationToken cancellationToken,
            Optional<IEnumerable<long>> excludedRoleIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default)
        {
            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable();

            if (excludedRoleIds.IsSpecified)
                query = query.Where(x => !excludedRoleIds.Value.Contains(x.RoleId));

            if (name.IsSpecified)
                query = query.Where(x => x.Name == name.Value);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            if (isLatestVersion.IsSpecified)
                query = isLatestVersion.Value
                    ? query.Where(x => x.NextVersionId == null)
                    : query.Where(x => x.NextVersionId != null);

            return query.AnyAsync(cancellationToken);
        }

        public IAsyncEnumerable<RoleIdentityViewModel> AsyncEnumerateIdentities(
                Optional<bool> isDeleted = default)
        {
            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            return query
                .Select(RoleIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<RolePermissionMappingIdentity> AsyncEnumeratePermissionMappingIdentities(
            long roleId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<RolePermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.RoleId == roleId);

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return query
                .Select(RolePermissionMappingIdentity.FromEntityProjection)
                .AsAsyncEnumerable();
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

        public async Task<IReadOnlyCollection<long>> CreatePermissionMappingsAsync(
            long roleId,
            IEnumerable<int> permissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var mappings = permissionIds
                .Select(permissionId => new RolePermissionMappingEntity()
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    CreationId = actionId
                })
                .ToArray();

            await _context.AddRangeAsync(mappings, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return mappings
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<OperationResult<RoleDetailViewModel>> ReadDetailAsync(
            CancellationToken cancellationToken,
            long roleId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null)
                .Where(x => x.RoleId == roleId);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            var result = await query
                .Select(RoleDetailViewModel.FromVersionEntityExpression)
                .FirstOrDefaultAsync(cancellationToken);

            return (result is null)
                ? new DataNotFoundError($"Role ID {roleId}").ToError<RoleDetailViewModel>()
                : result.ToSuccess();
        }

        public async Task<OperationResult<long>> UpdateAsync(
            CancellationToken cancellationToken,
            long roleId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var currentVersion = await _context.Set<RoleVersionEntity>()
                    .AsQueryable()
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

                if ((newVersion.Name == currentVersion.Name)
                        && (newVersion.IsDeleted == currentVersion.IsDeleted))
                {
                    transactionScope.Complete();
                    return new NoChangesGivenError($"Role ID {roleId}")
                        .ToError<long>();
                }

                currentVersion.NextVersion = newVersion;
                await _context.AddAsync(newVersion, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                transactionScope.Complete();

                return newVersion.Id
                    .ToSuccess();
            }
        }

        public async Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var findKeys = new object[1];
                foreach (var mappingId in mappingIds)
                {
                    findKeys[0] = mappingId;
                    var mapping = await _context.FindAsync<RolePermissionMappingEntity>(findKeys, cancellationToken);

                    mapping.DeletionId = deletionId;
                }

                await _context.SaveChangesAsync(cancellationToken);

                transactionScope.Complete();
            }
        }

        private readonly YastahDbContext _context;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IRolesRepository, RolesRepository>();
    }
}
