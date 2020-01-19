using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Permissions;
using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Authorization
{
    public interface IAuthorizationService
    {
        OperationResult RequireAuthentication();

        ValueTask<OperationResult> RequirePermissionsAsync(
            CancellationToken cancellationToken,
            params int[] permissionIds);
    }

    public class AuthorizationService
        : IAuthorizationService
    {
        public AuthorizationService(
            IAuthenticationService authenticationService,
            IPermissionsService permissionsService)
        {
            _authenticationService = authenticationService;
            _permissionsService = permissionsService;
        }

        public OperationResult RequireAuthentication()
            => _authenticationService.CurrentTicket is null
                ? new UnauthenticatedUserError()
                : OperationResult.Success;

        public async ValueTask<OperationResult> RequirePermissionsAsync(
            CancellationToken cancellationToken,
            params int[] permissionIds)
        {
            var result = RequireAuthentication();
            if (result.IsFailure)
                return result;

            var missingPermissionIds = permissionIds
                .Where(id => !_authenticationService.CurrentTicket!.GrantedPermissions
                    .ContainsKey(id))
                .ToHashSet();

            if (!missingPermissionIds.Any())
                return OperationResult.Success;

            return new InsufficientPermissionsError((await _permissionsService
                .GetIdentitiesAsync(cancellationToken))
                .Where(x => missingPermissionIds.Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.Name));
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IPermissionsService _permissionsService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IAuthorizationService, AuthorizationService>();
    }
}
