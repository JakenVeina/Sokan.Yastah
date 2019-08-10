using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Business.Permissions
{
    public interface IPermissionsOperations
    {
        Task<OperationResult<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>> GetDescriptionsAsync(
            CancellationToken cancellationToken);
    }

    public class PermissionsOperations
        : IPermissionsOperations
    {
        public PermissionsOperations(
            IAuthorizationService authorizationService,
            IPermissionsRepository permissionsRepository)
        {
            _authorizationService = authorizationService;
            _permissionsRepository = permissionsRepository;
        }

        public async Task<OperationResult<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>> GetDescriptionsAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManagePermissions);

            return (authResult.IsFailure)
                ? authResult.Error.ToError<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>()
                : (await _permissionsRepository.ReadDescriptionsAsync(cancellationToken))
                    .ToSuccess();
        }

        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionsRepository _permissionsRepository;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services, IConfiguration configuration)
            => services.AddScoped<IPermissionsOperations, PermissionsOperations>();
    }
}
