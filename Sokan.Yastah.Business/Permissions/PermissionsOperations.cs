using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Permissions
{
    public interface IPermissionsOperations
    {
        ValueTask<OperationResult<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>> GetDescriptionsAsync(
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class PermissionsOperations
        : IPermissionsOperations
    {
        public PermissionsOperations(
            IAuthorizationService authorizationService,
            IPermissionsService permissionsService)
        {
            _authorizationService = authorizationService;
            _permissionsService = permissionsService;
        }

        public async ValueTask<OperationResult<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>> GetDescriptionsAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManagePermissions);

            return authResult.IsFailure
                ? authResult.Error
                : (await _permissionsService.GetDescriptionsAsync(cancellationToken))
                    .ToSuccess();
        }

        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionsService _permissionsService;
    }
}
