using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        Task<IReadOnlyList<long>> ReadDefaultRoleIdsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> ReadDefaultPermissionIdsAsync(
            CancellationToken cancellationToken);

        Task<OperationResult<UserDetailViewModel>> ReadDetailAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PermissionIdentityViewModel>> ReadGrantedPermissionIdentitiesAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ulong>> ReadIdsAsync(
            CancellationToken cancellationToken,
            Optional<long> roleId = default);

        Task<IReadOnlyCollection<UserOverviewViewModel>> ReadOverviewsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<UserPermissionMappingIdentity>> ReadPermissionMappingIdentitiesAsync(
            CancellationToken cancellationToken,
            ulong userId,
            Optional<bool> isDeleted = default);

        Task<IReadOnlyCollection<UserRoleMappingIdentity>> ReadRoleMappingIdentitiesAsync(
            CancellationToken cancellationToken,
            ulong userId,
            Optional<bool> isDeleted = default);

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

        public async Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<int> permissionIds,
            PermissionMappingType type,
            long actionId,
            CancellationToken cancellationToken)
        {
            var entities = permissionIds
                .Select(permissionId => new UserPermissionMappingEntity()
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    IsDenied = type == PermissionMappingType.Denied,
                    CreationId = actionId
                })
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
                .Select(roleId => new UserRoleMappingEntity()
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreationId = actionId
                })
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
            using (var transactionScope = _transactionScopeFactory.CreateScope())
            {
                var result = MergeResult.SingleUpdate;

                var entity = await _context
                    .FindAsync<UserEntity>(new object[] { id }, cancellationToken);

                if (entity is null)
                {
                    result = MergeResult.SingleInsert;

                    entity = new UserEntity()
                    {
                        Id = id,
                        FirstSeen = firstSeen
                    };

                    await _context.AddAsync(entity, cancellationToken);
                }

                entity.Username = username;
                entity.Discriminator = discriminator;
                entity.AvatarHash = avatarHash;
                entity.LastSeen = lastSeen;

                await _context.SaveChangesAsync(cancellationToken);

                transactionScope.Complete();

                return result;
            }
        }

        public async Task<IReadOnlyList<long>> ReadDefaultRoleIdsAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<DefaultRoleMappingEntity>()
                .Where(x => x.DeletionId == null)
                .Select(x => x.RoleId)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyList<int>> ReadDefaultPermissionIdsAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<DefaultPermissionMappingEntity>()
                .Where(x => x.DeletionId == null)
                .Select(x => x.PermissionId)
                .ToArrayAsync(cancellationToken);

        public async Task<OperationResult<UserDetailViewModel>> ReadDetailAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            var result = await _context.Set<UserEntity>()
                .Where(x => x.Id == userId)
                .Select(UserDetailViewModel.FromEntityProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return (result is null)
                ? new DataNotFoundError($"User ID {userId}").ToError<UserDetailViewModel>()
                : result.ToSuccess();
        }

        public async Task<IReadOnlyCollection<PermissionIdentityViewModel>> ReadGrantedPermissionIdentitiesAsync(
                ulong userId,
                CancellationToken cancellationToken)
            => await _context
                .Set<PermissionEntity>()
                .Where(p => _context
                        .Set<UserPermissionMappingEntity>()
                        .Where(upm => upm.UserId == userId)
                        .Where(upm => !upm.IsDenied)
                        .Where(upm => upm.DeletionId == null)
                        .Any(upm => upm.PermissionId == p.PermissionId)
                    || _context
                        .Set<RolePermissionMappingEntity>()
                        .Where(rpm => _context
                            .Set<UserRoleMappingEntity>()
                            .Where(urm => urm.DeletionId == null)
                            .Where(urm => urm.UserId == userId)
                            .Any(urm => urm.RoleId == rpm.RoleId))
                        .Where(x => x.DeletionId == null)
                        .Any(rpm => rpm.PermissionId == p.PermissionId))
                .Where(p => !_context
                    .Set<UserPermissionMappingEntity>()
                    .Where(upm => upm.UserId == userId)
                    .Where(upm => upm.IsDenied)
                    .Where(upm => upm.DeletionId == null)
                    .Any(upm => upm.PermissionId == p.PermissionId))
                .Select(PermissionIdentityViewModel.FromEntityProjection)
                .ToArrayAsync();

        public async Task<IReadOnlyCollection<ulong>> ReadIdsAsync(
            CancellationToken cancellationToken,
            Optional<long> roleId = default)
        {
            var query = _context.Set<UserEntity>()
                .AsQueryable();

            if (roleId.IsSpecified)
                query = query.Where(u => u.RoleMappings.Any(urm =>
                    (urm.DeletionId == null)
                    && (urm.RoleId == roleId.Value)));

            return await query
                .Select(u => u.Id)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<UserOverviewViewModel>> ReadOverviewsAsync(
                CancellationToken cancellationToken)
            => await _context.Set<UserEntity>()
                .Select(UserOverviewViewModel.FromEntityProjection)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyCollection<UserPermissionMappingIdentity>> ReadPermissionMappingIdentitiesAsync(
            CancellationToken cancellationToken,
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<UserPermissionMappingEntity>()
                .Where(x => x.UserId == userId);

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return await query
                .Select(UserPermissionMappingIdentity.FromEntityProjection)
                .ToArrayAsync();
        }

        public async Task<IReadOnlyCollection<UserRoleMappingIdentity>> ReadRoleMappingIdentitiesAsync(
            CancellationToken cancellationToken,
            ulong userId,
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<UserRoleMappingEntity>()
                .Where(x => x.UserId == userId);

            if (isDeleted.IsSpecified)
                query = isDeleted.Value
                    ? query.Where(x => x.DeletionId != null)
                    : query.Where(x => x.DeletionId == null);

            return await query
                .Select(UserRoleMappingIdentity.FromEntityProjection)
                .ToArrayAsync();
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
                    var mapping = await _context.FindAsync<UserPermissionMappingEntity>(findKeys, cancellationToken);

                    mapping.DeletionId = deletionId;
                }

                await _context.SaveChangesAsync(cancellationToken);

                transactionScope.Complete();
            }
        }

        public async Task UpdateRoleMappingsAsync(
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
                    var mapping = await _context.FindAsync<UserRoleMappingEntity>(findKeys, cancellationToken);

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
            => services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
