using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                ? authResult.Error.ToError<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>()
                : (await _permissionsService.GetDescriptionsAsync(cancellationToken))
                    .ToSuccess();
        }

        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionsService _permissionsService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IPermissionsOperations, PermissionsOperations>();
    }
}
