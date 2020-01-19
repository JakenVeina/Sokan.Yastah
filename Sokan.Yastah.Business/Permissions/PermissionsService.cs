using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> GetDescriptionsAsync(
            CancellationToken cancellationToken);

        ValueTask<IReadOnlyCollection<PermissionIdentityViewModel>> GetIdentitiesAsync(
            CancellationToken cancellationToken);

        ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<int> permissionIds,
            CancellationToken cancellationToken);
    }

    public class PermissionsService
        : IPermissionsService
    {
        public PermissionsService(
            IMemoryCache memoryCache,
            IPermissionsRepository permissionsRepository)
        {
            _memoryCache = memoryCache;
            _permissionsRepository = permissionsRepository;
        }

        public ValueTask<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> GetDescriptionsAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(_getDescriptionsCacheKey, async entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;

                return await _permissionsRepository.AsyncEnumerateDescriptions()
                        .ToArrayAsync(cancellationToken)
                    as IReadOnlyCollection<PermissionCategoryDescriptionViewModel>;
            });

        public ValueTask<IReadOnlyCollection<PermissionIdentityViewModel>> GetIdentitiesAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(_getIdentitiesCacheKey, async entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;

                return await _permissionsRepository.AsyncEnumerateIdentities()
                        .ToArrayAsync(cancellationToken)
                    as IReadOnlyCollection<PermissionIdentityViewModel>;
            });

        public async ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<int> permissionIds,
            CancellationToken cancellationToken)
        {
            if (!permissionIds.Any())
                return OperationResult.Success;

            var invalidPermissionIds = permissionIds
                .Except((await GetIdentitiesAsync(cancellationToken))
                .Select(x => x.Id)).ToArray();
            
            return invalidPermissionIds.Any()
                ? ((invalidPermissionIds.Length == 1)
                    ? new DataNotFoundError($"Permission ID {invalidPermissionIds.First()}")
                    : new DataNotFoundError($"Permission IDs {string.Join(", ", invalidPermissionIds)}"))
                : OperationResult.Success;
        }

        private readonly IMemoryCache _memoryCache;
        private readonly IPermissionsRepository _permissionsRepository;

        internal const string _getDescriptionsCacheKey
            = nameof(PermissionsService) + "." + nameof(GetDescriptionsAsync);

        internal const string _getIdentitiesCacheKey
            = nameof(PermissionsService) + "." + nameof(GetIdentitiesAsync);

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<IPermissionsService, PermissionsService>();
    }
}
