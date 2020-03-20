using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Roles
{
    public interface IRolesRepository
    {
        Task<bool> AnyVersionsAsync(
            Optional<IEnumerable<long>> excludedRoleIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<RoleIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default);

        IAsyncEnumerable<RolePermissionMappingIdentityViewModel> AsyncEnumeratePermissionMappingIdentities(
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
            long roleId,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        Task<OperationResult<long>> UpdateAsync(
            long roleId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class RolesRepository
        : IRolesRepository
    {
        public RolesRepository(
            YastahDbContext context,
            ILogger<RolesRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyVersionsAsync(
            Optional<IEnumerable<long>> excludedRoleIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default)
        {
            RolesLogMessages.RoleVersionsEnumeratingAny(_logger, excludedRoleIds, name, isDeleted, isLatestVersion);

            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (excludedRoleIds.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(excludedRoleIds));
                query = query.Where(x => !excludedRoleIds.Value.Contains(x.RoleId));
            }

            if (name.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(name));
                query = query.Where(x => x.Name == name.Value);
            }

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            if (isLatestVersion.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isLatestVersion));
                query = isLatestVersion.Value
                    ? query.Where(x => x.NextVersionId == null)
                    : query.Where(x => x.NextVersionId != null);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query.AnyAsync(cancellationToken);

            RepositoryLogMessages.QueryExecuting(_logger);
            return result;
        }

        public IAsyncEnumerable<RoleIdentityViewModel> AsyncEnumerateIdentities(
                Optional<bool> isDeleted = default)
        {
            RolesLogMessages.RoleIdentitiesEnumerating(_logger, isDeleted);

            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(RoleIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<RolePermissionMappingIdentityViewModel> AsyncEnumeratePermissionMappingIdentities(
            long roleId,
            Optional<bool> isDeleted = default)
        {
            RolesLogMessages.RolePermissionMappingIdentitiesEnumerating(_logger, roleId, isDeleted);

            var query = _context.Set<RolePermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.RoleId == roleId);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(RolePermissionMappingIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken)
        {
            RolesLogMessages.RoleCreating(_logger, name, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var role = new RoleEntity(
                id: default);
            await _context.AddAsync(role, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new RoleVersionEntity(
                id: default,
                roleId: role.Id,
                name: name,
                isDeleted: false,
                creationId: actionId,
                previousVersionId: null,
                nextVersionId: null);
            await _context.AddAsync(version, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            RolesLogMessages.RoleCreated(_logger, role.Id, version.Id);
            return role.Id;
        }

        public async Task<IReadOnlyCollection<long>> CreatePermissionMappingsAsync(
            long roleId,
            IEnumerable<int> permissionIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            RolesLogMessages.RolePermissionMappingsCreating(_logger, roleId, permissionIds, actionId);

            var mappings = permissionIds
                .Do(permissionId => RolesLogMessages.RolePermissionMappingCreating(_logger, roleId, permissionId, actionId))
                .Select(permissionId => new RolePermissionMappingEntity(
                    id:             default,
                    roleId:         roleId,
                    permissionId:   permissionId,
                    creationId:     actionId,
                    deletionId:     null))
                .ToArray();
            await _context.AddRangeAsync(mappings, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var mappingIds = mappings
                .Select(x => x.Id)
                .Do(mappingId => RolesLogMessages.RolePermissionMappingCreated(_logger, mappingId))
                .ToArray();

            RolesLogMessages.RolePermissionMappingsCreated(_logger);
            return mappingIds;
        }

        public async Task<OperationResult<RoleDetailViewModel>> ReadDetailAsync(
            long roleId,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            RolesLogMessages.RoleDetailReading(_logger, roleId, isDeleted);

            var query = _context.Set<RoleVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null)
                .Where(x => x.RoleId == roleId);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryExecuting(_logger);
            var result = await query
                .Select(RoleDetailViewModel.FromVersionEntityExpression)
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                RolesLogMessages.RoleVersionNotFound(_logger, roleId);
                return new DataNotFoundError($"Role ID {roleId}");
            }
            else
            {
                RolesLogMessages.RoleDetailRead(_logger, roleId);
                return result.ToSuccess();
            }
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long roleId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            RolesLogMessages.RoleUpdating(_logger, roleId, actionId, name, isDeleted);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);
            
            var currentVersion = await _context.Set<RoleVersionEntity>()
                .AsQueryable()
                .Where(x => x.RoleId == roleId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
            {
                RolesLogMessages.RoleVersionNotFound(_logger, roleId);
                return new DataNotFoundError($"Role ID {roleId}");
            }

            var newVersion = new RoleVersionEntity(
                id: default,
                roleId: currentVersion.RoleId,
                name: name.IsSpecified
                                        ? name.Value
                                        : currentVersion.Name,
                isDeleted: isDeleted.IsSpecified
                                        ? isDeleted.Value
                                        : currentVersion.IsDeleted,
                creationId: actionId,
                previousVersionId: currentVersion.Id,
                nextVersionId: null
            );

            if ((newVersion.Name == currentVersion.Name)
                    && (newVersion.IsDeleted == currentVersion.IsDeleted))
            {
                TransactionsLogMessages.TransactionScopeCommitted(_logger);
                transactionScope.Complete();

                RolesLogMessages.RoleNoChangesGiven(_logger, roleId);
                return new NoChangesGivenError($"Role ID {roleId}");
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            RolesLogMessages.RoleUpdated(_logger, roleId, newVersion.Id);
            return newVersion.Id
                .ToSuccess();
        }

        public async Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            RolesLogMessages.RolePermissionMappingsUpdating(_logger, mappingIds, deletionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            foreach (var mappingId in mappingIds)
            {
                RolesLogMessages.RolePermissionMappingUpdating(_logger, mappingId, deletionId);
                var mapping = await _context.FindAsync<RolePermissionMappingEntity?>(new object[] { mappingId }, cancellationToken);

                mapping!.DeletionId = deletionId;
            }

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            RolesLogMessages.RolePermissionMappingsUpdated(_logger);
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
