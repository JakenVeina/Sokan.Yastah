using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class PermissionsService
        : IPermissionsService
    {
        public PermissionsService(
            ILogger<PermissionsService> logger,
            IMemoryCache memoryCache,
            IPermissionsRepository permissionsRepository)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _permissionsRepository = permissionsRepository;
        }

        public ValueTask<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>> GetDescriptionsAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>(_getDescriptionsCacheKey, async entry =>
            {
                PermissionsLogMessages.PermissionCategoryDescriptionsFetching(_logger);

                entry.Priority = CacheItemPriority.NeverRemove;

                var descriptions = await _permissionsRepository.AsyncEnumerateDescriptions()
                    .ToArrayAsync(cancellationToken);
                PermissionsLogMessages.PermissionCategoryDescriptionsFetched(_logger);

                return descriptions;
            });

        public ValueTask<IReadOnlyCollection<PermissionIdentityViewModel>> GetIdentitiesAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync<IReadOnlyCollection<PermissionIdentityViewModel>>(_getIdentitiesCacheKey, async entry =>
            {
                PermissionsLogMessages.PermissionIdentitiesFetching(_logger);
                
                entry.Priority = CacheItemPriority.NeverRemove;

                var identities = await _permissionsRepository.AsyncEnumerateIdentities()
                    .ToArrayAsync(cancellationToken);
                PermissionsLogMessages.PermissionIdentitiesFetched(_logger);

                return identities;
            });

        public async ValueTask<OperationResult> ValidateIdsAsync(
            IReadOnlyCollection<int> permissionIds,
            CancellationToken cancellationToken)
        {
            PermissionsLogMessages.PermissionIdsValidating(_logger, permissionIds);

            if (!permissionIds.Any())
            {
                PermissionsLogMessages.PermissionIdsValidationSucceeded(_logger, permissionIds);
                return OperationResult.Success;
            }

            var invalidPermissionIds = permissionIds
                .Except((await GetIdentitiesAsync(cancellationToken))
                .Select(x => x.Id)).ToArray();
            
            if (invalidPermissionIds.Length == 0)
            {
                PermissionsLogMessages.PermissionIdsValidationSucceeded(_logger, permissionIds);
                return OperationResult.Success;
            }
            else if (invalidPermissionIds.Length == 1)
            {
                PermissionsLogMessages.PermissionIdsValidationFailed(_logger, invalidPermissionIds);
                return new DataNotFoundError($"Permission ID {invalidPermissionIds.First()}");
            }
            else
            {
                PermissionsLogMessages.PermissionIdsValidationFailed(_logger, invalidPermissionIds);
                return new DataNotFoundError($"Permission IDs {string.Join(", ", invalidPermissionIds)}");
            }
        }

        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IPermissionsRepository _permissionsRepository;

        internal const string _getDescriptionsCacheKey
            = nameof(PermissionsService) + "." + nameof(GetDescriptionsAsync);

        internal const string _getIdentitiesCacheKey
            = nameof(PermissionsService) + "." + nameof(GetIdentitiesAsync);
    }
}
