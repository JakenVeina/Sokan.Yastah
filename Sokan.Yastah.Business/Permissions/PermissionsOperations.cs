using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            ILogger<PermissionsOperations> logger,
            IPermissionsService permissionsService)
        {
            _authorizationService = authorizationService;
            _logger = logger;
            _permissionsService = permissionsService;
        }

        public async ValueTask<OperationResult<IReadOnlyCollection<PermissionCategoryDescriptionViewModel>>> GetDescriptionsAsync(
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManagePermissions },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var result = (await _permissionsService.GetDescriptionsAsync(cancellationToken))
                .ToSuccess();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IPermissionsService _permissionsService;
    }
}
