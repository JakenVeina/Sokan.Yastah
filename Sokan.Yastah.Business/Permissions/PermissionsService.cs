using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Permissions
{
    public interface IPermissionsService
    {
        ValueTask<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> GetDescriptionsAsync(
            CancellationToken cancellationToken);

        ValueTask<IReadOnlyCollection<PermissionIdentity>> GetIdentitiesAsync(
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
            => _memoryCache.GetOrCreateLongTermAsync(_getDescriptionsCacheKey, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;

                return _permissionsRepository.ReadDescriptionsAsync(cancellationToken);
            });

        public ValueTask<IReadOnlyCollection<PermissionIdentity>> GetIdentitiesAsync(
                CancellationToken cancellationToken)
            => _memoryCache.GetOrCreateLongTermAsync(_getIdentitiesCacheKey, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;

                return _permissionsRepository.ReadIdentitiesAsync(cancellationToken);
            });

        private readonly IMemoryCache _memoryCache;
        private readonly IPermissionsRepository _permissionsRepository;

        private const string _getDescriptionsCacheKey
            = nameof(PermissionsService) + "." + nameof(GetDescriptionsAsync);

        private const string _getIdentitiesCacheKey
            = nameof(PermissionsService) + "." + nameof(GetIdentitiesAsync);

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services
                .AddScoped<IPermissionsService, PermissionsService>();
    }
}
