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
using Sokan.Yastah.Data.Concurrency;
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

        // TODO: try combining GetDefaultRoleIds and GetDefaultPermissionIds into one query
        Task<IReadOnlyList<long>> GetDefaultRoleIdsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<int>> GetDefaultPermissionIdsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PermissionIdentity>> GetGrantedPermissionIdentitiesAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<MergeResult> MergeAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            CancellationToken cancellationToken);
    }

    public class UsersRepository
        : IUsersRepository
    {
        public UsersRepository(
            IConcurrencyResolutionService concurrencyResolutionService,
            YastahDbContext context,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _concurrencyResolutionService = concurrencyResolutionService;
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
                });

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
                });

            await _context.AddRangeAsync(entities, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entities
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<IReadOnlyList<long>> GetDefaultRoleIdsAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<DefaultRoleMappingEntity>()
                .AsNoTracking()
                .Where(x => x.DeletionId == null)
                .Select(x => x.RoleId)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyList<int>> GetDefaultPermissionIdsAsync(
                CancellationToken cancellationToken)
            => await _context
                .Set<DefaultPermissionMappingEntity>()
                .AsNoTracking()
                .Where(x => x.DeletionId == null)
                .Select(x => x.PermissionId)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyCollection<PermissionIdentity>> GetGrantedPermissionIdentitiesAsync(
                ulong userId,
                CancellationToken cancellationToken)
            => await _context
                .Set<PermissionEntity>()
                .AsNoTracking()
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
                    .Where(upm => !upm.IsDenied)
                    .Where(upm => upm.DeletionId == null)
                    .Any(upm => upm.PermissionId == p.PermissionId))
                .Select(PermissionIdentity.FromEntityProjection)
                .ToArrayAsync();

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
                    .Set<UserEntity>()
                    .FindAsync(new object[] { id }, cancellationToken);

                if (entity is null)
                {
                    result = MergeResult.SingleInsert;

                    entity = new UserEntity()
                    {
                        Id = id,
                        FirstSeen = firstSeen
                    };

                    await _context
                        .Set<UserEntity>()
                        .AddAsync(entity);
                }

                entity.Username = username;
                entity.Discriminator = discriminator;
                entity.AvatarHash = avatarHash;
                entity.LastSeen = lastSeen;

                await _concurrencyResolutionService
                    .SaveConcurrentChangesAsync(_context, cancellationToken);

                transactionScope.Complete();

                return result;
            }
        }

        private readonly IConcurrencyResolutionService _concurrencyResolutionService;
        private readonly YastahDbContext _context;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IUsersRepository, UsersRepository>();
    }
}
