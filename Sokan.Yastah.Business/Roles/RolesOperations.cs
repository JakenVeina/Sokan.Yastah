using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Business.Roles
{
    public interface IRolesOperations
    {
        Task<OperationResult<long>> CreateAsync(
            RoleCreationModel creationModel,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long roleId,
            CancellationToken cancellationToken);

        Task<OperationResult<RoleDetailViewModel>> GetDetailAsync(
            long roleId,
            CancellationToken cancellationToken);

        ValueTask<OperationResult<IReadOnlyCollection<RoleIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class RolesOperations
        : IRolesOperations
    {
        public RolesOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ILogger<RolesOperations> logger,
            IRolesRepository rolesRepository,
            IRolesService rolesService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _rolesRepository = rolesRepository;
            _rolesService = rolesService;
        }

        public async Task<OperationResult<long>> CreateAsync(
            RoleCreationModel creationModel,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManageRoles },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _rolesService.CreateAsync(creationModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> DeleteAsync(
            long roleId,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManageRoles },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _rolesService.DeleteAsync(roleId, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult<RoleDetailViewModel>> GetDetailAsync(
            long roleId,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManageRoles },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var result = await _rolesRepository.ReadDetailAsync(
                    roleId: roleId,
                    isDeleted: false,
                    cancellationToken: cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async ValueTask<OperationResult<IReadOnlyCollection<RoleIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManageRoles },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var result =(await _rolesService.GetCurrentIdentitiesAsync(cancellationToken))
                .ToSuccess();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger, this);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)AdministrationPermission.ManageRoles },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _rolesService.UpdateAsync(roleId, updateModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IRolesRepository _rolesRepository;
        private readonly IRolesService _rolesService;
    }
}
