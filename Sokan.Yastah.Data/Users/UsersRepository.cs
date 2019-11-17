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

using Sokan.Yastah.Data.Permissions;
using Sokan.Yastah.Data.Roles;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Users
{
    public interface IUsersRepository
    {
        Task<bool> AnyAsync(
            CancellationToken cancellationToken,
            Optional<ulong> userId = default);

        IAsyncEnumerable<long> AsyncEnumerateDefaultRoleIds();

        IAsyncEnumerable<int> AsyncEnumerateDefaultPermissionIds();

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

    public class UsersRepository
        : IUsersRepository
    {
        public UsersRepository(
            YastahDbContext context,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyAsync(
            CancellationToken cancellationToken,
            Optional<ulong> userId = default)
        {
            var query = _context.Set<UserEntity>()
                .AsQueryable();

            if (userId.IsSpecified)
                query = query.Where(x => x.Id == userId.Value);

            return query.AnyAsync(cancellationToken);
        }

        public IAsyncEnumerable<long> AsyncEnumerateDefaultRoleIds()
            => _context.Set<DefaultRoleMappingEntity>()
                .AsQueryable()
                .Where(x => x.DeletionId == null)
                .Select(x => x.RoleId)
                .AsAsyncEnumerable();

        public IAsyncEnumerable<int> AsyncEnumerateDefaultPermissionIds()
            => _context.Set<DefaultPermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.DeletionId == null)
                .Select(x => x.PermissionId)
                .AsAsyncEnumerable();

        public IAsyncEnumerable<PermissionIdentityViewModel> AsyncEnumerateGrantedPermissionIdentities(
                ulong userId)
            => _context.Set<PermissionEntity>()
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

        public IAsyncEnumerable<ulong> AsyncEnumerateIds(
            Optional<long> roleId = default)
        {
            var query = _context.Set<UserEntity>()
                .AsQueryable();

            if (roleId.IsSpecified)
                query = query.Where(u => u.RoleMappings.Any(urm =>
                    (urm.DeletionId == null)
                    && (urm.RoleId == roleId.Value)));

            return query
                .Select(u => u.Id)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<UserOverviewViewModel> AsyncEnumerateOverviews()
            => _context.Set<UserEntity>()
                .Select(UserOverviewViewModel.FromEntityProjection)
                .AsAsyncEnumerable();

        public IAsyncEnumerable<UserPermissionMappingIdentityViewModel> AsyncEnumeratePermissionMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<UserPermissionMappingEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId);

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return query
                .Select(UserPermissionMappingIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<UserRoleMappingIdentityViewModel> AsyncEnumerateRoleMappingIdentities(
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<UserRoleMappingEntity>()
                .AsQueryable()
                .Where(x => x.UserId == userId);

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return query
                .Select(UserRoleMappingIdentityViewModel.FromEntityProjection)
                .AsAsyncEnumerable();
        }

        public async Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<int> permissionIds,
            PermissionMappingType type,
            long actionId,
            CancellationToken cancellationToken)
        {
            var entities = permissionIds
                .Select(permissionId => new UserPermissionMappingEntity(
                    id:             default,
                    userId:         userId,
                    permissionId:   permissionId,
                    isDenied:       type == PermissionMappingType.Denied,
                    creationId:     actionId,
                    deletionId:     null))
                .ToArray();

            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entities
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<IReadOnlyList<long>> CreateRoleMappingsAsync(
            ulong userId,
            IEnumerable<long> roleIds,
            long actionId,
            CancellationToken cancellationToken)
        {
            var entities = roleIds
                .Select(roleId => new UserRoleMappingEntity(
                    id:         default,
                    userId:     userId,
                    roleId:     roleId,
                    creationId: actionId,
                    deletionId: null))
                .ToArray();

            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entities
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<MergeResult> MergeAsync(
            ulong id,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();
            
            var result = MergeResult.SingleUpdate;

            var entity = await _context
                .FindAsync<UserEntity?>(new object[] { id }, cancellationToken);

            if (entity is null)
            {
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
                entity.Username = username;
                entity.Discriminator = discriminator;
                entity.AvatarHash = avatarHash;
                entity.LastSeen = lastSeen;
            }

            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return result;
        }

        public async Task<OperationResult<UserDetailViewModel>> ReadDetailAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            var result = await _context.Set<UserEntity>()
                .AsQueryable()
                .Where(x => x.Id == userId)
                .Select(UserDetailViewModel.FromEntityProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return (result is null)
                ? new DataNotFoundError($"User ID {userId}").ToError<UserDetailViewModel>()
                : result.ToSuccess();
        }

        public async Task UpdatePermissionMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();
            
            var findKeys = new object[1];
            foreach (var mappingId in mappingIds)
            {
                findKeys[0] = mappingId;
                var mapping = await _context.FindAsync<UserPermissionMappingEntity>(findKeys, cancellationToken);

                mapping.DeletionId = deletionId;
            }

            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }

        public async Task UpdateRoleMappingsAsync(
            IEnumerable<long> mappingIds,
            long deletionId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();
            
            var findKeys = new object[1];
            foreach (var mappingId in mappingIds)
            {
                findKeys[0] = mappingId;
                var mapping = await _context.FindAsync<UserRoleMappingEntity>(findKeys, cancellationToken);

                mapping.DeletionId = deletionId;
            }

            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }

        private readonly YastahDbContext _context;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
