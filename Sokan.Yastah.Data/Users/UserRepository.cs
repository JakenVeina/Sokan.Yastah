using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.Messaging;
using Sokan.Yastah.Data.Authorization;

namespace Sokan.Yastah.Data.Users
{
    public interface IUserRepository
    {
        Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<long> permissionIds,
            PermissionMappingType type,
            ulong createdById,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<long>> CreateRoleMappingsAsync(
            ulong userId,
            IEnumerable<long> roleIds,
            ulong createdById,
            CancellationToken cancellationToken);

        // TODO: try combining GetDefaultRoleIds and GetDefaultPermissionIds into one query
        Task<IReadOnlyList<long>> GetDefaultRoleIdsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyList<long>> GetDefaultPermissionIdsAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PermissionIdentity>> GetGrantedPermissionIdentitiesAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<MergeResult> MergeAsync(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            CancellationToken cancellationToken);
    }

    public class UserRepository
        : IUserRepository
    {
        public UserRepository(
            IConcurrencyResolutionService concurrencyResolutionService,
            IMessenger messenger,
            ISystemClock systemClock,
            YastahDbContext yastahDbContext)
        {
            _concurrencyResolutionService = concurrencyResolutionService;
            _messenger = messenger;
            _systemClock = systemClock;
            _yastahDbContext = yastahDbContext;
        }

        public async Task<IReadOnlyList<long>> CreatePermissionMappingsAsync(
            ulong userId,
            IEnumerable<long> permissionIds,
            PermissionMappingType type,
            ulong createdById,
            CancellationToken cancellationToken)
        {
            var entities = permissionIds
                .Select(permissionId => new UserPermissionMappingEntity()
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    IsDenied = type == PermissionMappingType.Denied,
                    Created = _systemClock.UtcNow,
                    CreatedById = createdById
                });

            await _yastahDbContext
                .UserPermissionMappings
                .AddRangeAsync(entities);

            await _yastahDbContext
                .SaveChangesAsync();

            return entities
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<IReadOnlyList<long>> CreateRoleMappingsAsync(
            ulong userId,
            IEnumerable<long> roleIds,
            ulong createdById,
            CancellationToken cancellationToken)
        {
            var entities = roleIds
                .Select(roleId => new UserRoleMappingEntity()
                {
                    UserId = userId,
                    RoleId = roleId,
                    Created = _systemClock.UtcNow,
                    CreatedById = createdById
                });

            await _yastahDbContext
                .UserRoleMappings
                .AddRangeAsync(entities);

            await _yastahDbContext
                .SaveChangesAsync();

            return entities
                .Select(x => x.Id)
                .ToArray();
        }

        public async Task<IReadOnlyList<long>> GetDefaultRoleIdsAsync(
                CancellationToken cancellationToken)
            => await _yastahDbContext
                .UserRoleDefaultMappings
                .Where(x => x.DeletedById == null)
                .Select(x => x.RoleId)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyList<long>> GetDefaultPermissionIdsAsync(
                CancellationToken cancellationToken)
            => await _yastahDbContext
                .UserPermissionDefaultMappings
                .Where(x => x.DeletedById == null)
                .Select(x => x.PermissionId)
                .ToArrayAsync(cancellationToken);

        public async Task<IReadOnlyCollection<PermissionIdentity>> GetGrantedPermissionIdentitiesAsync(
                ulong userId,
                CancellationToken cancellationToken)
            => await _yastahDbContext
                .Permissions
                .Where(p => _yastahDbContext
                        .UserPermissionMappings
                        .Where(upm => upm.UserId == userId)
                        .Where(upm => !upm.IsDenied)
                        .Where(upm => upm.DeletedById == null)
                        .Any(upm => upm.PermissionId == p.PermissionId)
                    || _yastahDbContext
                        .RolePermissionMappings
                        .Where(rpm => _yastahDbContext
                            .UserRoleMappings
                            .Where(urm => urm.DeletedById == null)
                            .Where(urm => urm.UserId == userId)
                            .Any(urm => urm.RoleId == rpm.RoleId))
                        .Where(x => x.DeletedById == null)
                        .Any(rpm => rpm.PermissionId == p.PermissionId))
                .Where(p => !_yastahDbContext
                    .UserPermissionMappings
                    .Where(upm => upm.UserId == userId)
                    .Where(upm => !upm.IsDenied)
                    .Where(upm => upm.DeletedById == null)
                    .Any(upm => upm.PermissionId == p.PermissionId))
                .Select(PermissionIdentity.FromEntityProjection)
                .ToArrayAsync();

        public async Task<MergeResult> MergeAsync(
            ulong id,
            string username,
            string discriminator,
            string avatarHash,
            CancellationToken cancellationToken)
        {
            var result = MergeResult.SingleUpdate;

            var entity = await _yastahDbContext
                .Users
                .FindAsync(new object[] { id }, cancellationToken);

            var now = _systemClock.UtcNow;

            if (entity is null)
            {
                result = MergeResult.SingleInsert;

                entity = new UserEntity()
                {
                    Id = id,
                    FirstSeen = now
                };

                await _yastahDbContext
                    .Users
                    .AddAsync(entity);
            }

            entity.Username = username;
            entity.Discriminator = discriminator;
            entity.AvatarHash = avatarHash;
            entity.LastSeen = now;

            await _concurrencyResolutionService
                .SaveConcurrentChangesAsync(_yastahDbContext, cancellationToken);

            if(result.RowsInserted > 0)
                await _messenger.PublishNotificationAsync(
                    new UserCreatedNotification(
                        id,
                        username,
                        discriminator,
                        avatarHash),
                    cancellationToken);

            return result;
        }

        private readonly IConcurrencyResolutionService _concurrencyResolutionService;

        private readonly IMessenger _messenger;

        private readonly ISystemClock _systemClock;

        private readonly YastahDbContext _yastahDbContext;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IUserRepository, UserRepository>();
    }
}
