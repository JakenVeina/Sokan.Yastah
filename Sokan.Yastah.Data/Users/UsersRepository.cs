using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Users
{
    public interface IUsersRepository
    {
        Task<bool> AnyAsync(
            Optional<ulong> userId = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<int> AsyncEnumerateDefaultPermissionIds();

        IAsyncEnumerable<long> AsyncEnumerateDefaultRoleIds();

        IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateGrantedPermissionIdentities(
            ulong userId);

        IAsyncEnumerable<ulong> AsyncEnumerateIds(
            Optional<long> roleId = default);

        IAsyncEnumerable<UserOverviewViewModel> AsyncEnumerateOverviews();

        IAsyncEnumerable<UserPermissionMappingIdentityViewModel> AsyncEnumeratePermissionMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default);

        IAsyncEnumerable<UserRoleMappingIdentityViewModel> AsyncEnumerateRoleMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default);

        Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<int> permissionIds,
            PermissionMappingType type,
            long actionId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<long>> CreateRoleMappingsAsync(
            ulong userId,
            IEnumerable<long> roleIds,
            long actionId,
            CancellationToken cancellationToken);

        Task<MergeResult> MergeAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            CancellationToken cancellationToken);

        Task<OperationResult<UserDetailViewModel>> ReadDetailAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken);

        Task UpdateRoleMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class UsersRepository
        : IUsersRepository
    {
        public UsersRepository(
            YastahDbContext context,
            ILogger<UsersRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyAsync(
            Optional<ulong> userId = default,
            CancellationToken cancellationToken = default)
        {
            UsersLogMessages.UsersEnumeratingAny(_logger, userId);

            var query = _context.Set<UserEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (userId.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(userId));
                query = query.Where(x => x.Id == userId.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query.AnyAsync(cancellationToken);

            RepositoryLogMessages.QueryExecuting(_logger);
            return result;
        }

        public IAsyncEnumerable<int> AsyncEnumerateDefaultPermissionIds()
        {
            UsersLogMessages.DefaultPermissionIdsEnumerating(_logger);

            var result = _context.Set<DefaultPermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.DeletionId == null)
                .Select(x => x.PermissionId)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<long> AsyncEnumerateDefaultRoleIds()
        {
            UsersLogMessages.DefaultRoleIdsEnumerating(_logger);

            var result = _context.Set<DefaultRoleMappingEntity>()
                .AsQueryable()
                .Where(x => x.DeletionId == null)
                .Select(x => x.RoleId)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateGrantedPermissionIdentities(
                ulong userId)
        {
            UsersLogMessages.GrantedPermissionIdentitiesEnumerating(_logger, userId);

            var result = _context.Set<PermissionEntity>()
                .AsQueryable()
                .Where(p => _context.Set<UserPermissionMappingEntity>()
                        .AsQueryable()
                        .Where(upm => upm.UserId == userId)
                        .Where(upm => !upm.IsDenied)
                        .Where(upm => upm.DeletionId == null)
                        .Any(upm => upm.PermissionId == p.PermissionId)
                    || _context.Set<RolePermissionMappingEntity>()
                        .AsQueryable()
                        .Where(rpm => _context.Set<UserRoleMappingEntity>()
                            .AsQueryable()
                            .Where(urm => urm.DeletionId == null)
                            .Where(urm => urm.UserId == userId)
                            .Any(urm => urm.RoleId == rpm.RoleId))
                        .Where(x => x.DeletionId == null)
                        .Any(rpm => rpm.PermissionId == p.PermissionId))
                .Where(p => !_context.Set<UserPermissionMappingEntity>()
                    .AsQueryable()
                    .Where(upm => upm.UserId == userId)
                    .Where(upm => upm.IsDenied)
                    .Where(upm => upm.DeletionId == null)
                    .Any(upm => upm.PermissionId == p.PermissionId))
                .Select(PermissionIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<ulong> AsyncEnumerateIds(
            Optional<long> roleId = default)
        {
            UsersLogMessages.UserIdsEnumerating(_logger, roleId);

            var query = _context.Set<UserEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (roleId.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(roleId));
                query = query.Where(u => u.RoleMappings.Any(urm =>
                    (urm.DeletionId == null)
                    && (urm.RoleId == roleId.Value)));
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(u => u.Id)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<UserOverviewViewModel> AsyncEnumerateOverviews()
        {
            UsersLogMessages.UserOverviewsEnumerating(_logger);

            var result = _context.Set<UserEntity>()
                .Select(UserOverviewViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<UserPermissionMappingIdentityViewModel> AsyncEnumeratePermissionMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            UsersLogMessages.UserPermissionMappingIdentitiesEnumerating(_logger, userId, isDeleted);

            var query = _context.Set<UserPermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId);
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
                .Select(UserPermissionMappingIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public IAsyncEnumerable<UserRoleMappingIdentityViewModel> AsyncEnumerateRoleMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            UsersLogMessages.UserRoleMappingIdentitiesEnumerating(_logger, userId, isDeleted);

            var query = _context.Set<UserRoleMappingEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId);
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
                .Select(UserRoleMappingIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<int> permissionIds,
            PermissionMappingType type,
            long actionId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserPermissionMappingsCreating(_logger, userId, permissionIds, type, actionId);

            var entities = permissionIds
                .Do(permissionId => UsersLogMessages.UserPermissionMappingCreating(_logger, userId, permissionId, type, actionId))
                .Select(permissionId => new UserPermissionMappingEntity(
                    id:             default,
                    userId:         userId,
                    permissionId:   permissionId,
                    isDenied:       type == PermissionMappingType.Denied,
                    creationId:     actionId,
                    deletionId:     null))
                .ToArray();
            await _context.AddRangeAsync(entities, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var mappingIds = entities
                .Select(x => x.Id)
                .Do(mappingId => UsersLogMessages.UserPermissionMappingCreated(_logger, mappingId))
                .ToArray();

            UsersLogMessages.UserPermissionMappingsCreated(_logger);
            return mappingIds;
        }

        public async Task<IReadOnlyList<long>> CreateRoleMappingsAsync(
            ulong userId,
            IEnumerable<long> roleIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserRoleMappingsCreating(_logger, userId, roleIds, actionId);

            var entities = roleIds
                .Do(roleId => UsersLogMessages.UserRoleMappingCreating(_logger, userId, roleId, actionId))
                .Select(roleId => new UserRoleMappingEntity(
                    id:         default,
                    userId:     userId,
                    roleId:     roleId,
                    creationId: actionId,
                    deletionId: null))
                .ToArray();
            await _context.AddRangeAsync(entities, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var mappingIds = entities
                .Select(x => x.Id)
                .Do(mappingId => UsersLogMessages.UserRoleMappingCreated(_logger, mappingId))
                .ToArray();

            UsersLogMessages.UserRoleMappingsCreated(_logger);
            return mappingIds;
        }

        public async Task<MergeResult> MergeAsync(
            ulong id,
            string username,
            string discriminator,
            string? avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserMerging(_logger, id, username, discriminator, avatarHash, firstSeen, lastSeen);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);
            
            var result = MergeResult.SingleUpdate;

            var entity = await _context
                .FindAsync<UserEntity?>(new object[] { id }, cancellationToken);

            if (entity is null)
            {
                UsersLogMessages.UserCreating(_logger, id, username, discriminator, avatarHash, firstSeen, lastSeen);
                result = MergeResult.SingleInsert;

                entity = new UserEntity(
                    id,
                    username,
                    discriminator,
                    avatarHash,
                    firstSeen,
                    lastSeen);
                await _context.AddAsync(entity, cancellationToken);
            }
            else
            {
                UsersLogMessages.UserUpdating(_logger, id, username, discriminator, avatarHash, lastSeen);

                entity.Username = username;
                entity.Discriminator = discriminator;
                entity.AvatarHash = avatarHash;
                entity.LastSeen = lastSeen;
            }

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            UsersLogMessages.UserMerged(_logger, id);
            return result;
        }

        public async Task<OperationResult<UserDetailViewModel>> ReadDetailAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserDetailReading(_logger, userId);

            var result = await _context.Set<UserEntity>()
                .AsQueryable()
                .Where(x => x.Id == userId)
                .Select(UserDetailViewModel.FromEntityProjection)
                .FirstOrDefaultAsync(cancellationToken);
            RepositoryLogMessages.QueryExecuting(_logger);

            if (result is null)
            {
                UsersLogMessages.UserNotFound(_logger, userId);
                return new DataNotFoundError($"User ID {userId}");
            }
            else
            {
                UsersLogMessages.UserDetailRead(_logger, userId);
                return result.ToSuccess();
            }
        }

        public async Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserPermissionMappingsUpdating(_logger, mappingIds, deletionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            foreach (var mappingId in mappingIds)
            {
                UsersLogMessages.UserPermissionMappingUpdating(_logger, mappingId, deletionId);
                var mapping = await _context.FindAsync<UserPermissionMappingEntity?>(new object[] { mappingId }, cancellationToken);

                mapping!.DeletionId = deletionId;
            }

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            UsersLogMessages.UserPermissionMappingsUpdated(_logger);
        }

        public async Task UpdateRoleMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            UsersLogMessages.UserRoleMappingsUpdating(_logger, mappingIds, deletionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            foreach (var mappingId in mappingIds)
            {
                UsersLogMessages.UserRoleMappingUpdating(_logger, mappingId, deletionId);
                var mapping = await _context.FindAsync<UserRoleMappingEntity?>(new object[] { mappingId }, cancellationToken);

                mapping!.DeletionId = deletionId;
            }

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            UsersLogMessages.UserRoleMappingsUpdated(_logger);
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
