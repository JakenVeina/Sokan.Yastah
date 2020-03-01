using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Authorization
{
    public interface IAuthorizationService
    {
        OperationResult RequireAuthentication();

        ValueTask<OperationResult> RequirePermissionsAsync(
            IReadOnlyCollection<int> permissionIds,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class AuthorizationService
        : IAuthorizationService
    {
        public AuthorizationService(
            IAuthenticationService authenticationService,
            ILogger<AuthorizationService> logger,
            IPermissionsService permissionsService)
        {
            _authenticationService = authenticationService;
            _logger = logger;
            _permissionsService = permissionsService;
        }

        public OperationResult RequireAuthentication()
        {
            using var logScope = _logger.BeginMemberScope();
            AuthorizationLogMessages.AuthenticationRequired(_logger);
            
            if(_authenticationService.CurrentTicket is null)
            {
                AuthorizationLogMessages.AuthenticatedUserNotFound(_logger);
                return new UnauthenticatedUserError();
            }
            else
            {
                AuthorizationLogMessages.AuthenticatedUserFound(_logger, _authenticationService.CurrentTicket.UserId);
                return OperationResult.Success;
            }
        }

        public async ValueTask<OperationResult> RequirePermissionsAsync(
            IReadOnlyCollection<int> permissionIds,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            AuthorizationLogMessages.PermissionsRequired(_logger, permissionIds);

            var authenticationResult = RequireAuthentication();
            if (authenticationResult.IsFailure)
                return authenticationResult;

            var missingPermissionIds = permissionIds
                .Where(id => !_authenticationService.CurrentTicket!.GrantedPermissions
                    .ContainsKey(id))
                .ToHashSet();

            if (!missingPermissionIds.Any())
            {
                AuthorizationLogMessages.RequiredPermissionsFound(_logger);
                return OperationResult.Success;
            }
            AuthorizationLogMessages.RequiredPermissionsNotFound(_logger, missingPermissionIds);

            var missingPermissions = (await _permissionsService
                .GetIdentitiesAsync(cancellationToken))
                .Where(x => missingPermissionIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.Name);
            AuthorizationLogMessages.MissingPermissionsFetched(_logger, missingPermissions);

            return new InsufficientPermissionsError(missingPermissions);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger _logger;
        private readonly IPermissionsService _permissionsService;
    }
}
