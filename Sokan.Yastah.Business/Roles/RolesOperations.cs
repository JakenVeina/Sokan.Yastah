using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        Task<OperationResult<IReadOnlyCollection<RoleIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    public class RolesOperations
        : IRolesOperations
    {
        public RolesOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            IRolesRepository rolesRepository,
            IRolesService rolesService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _rolesRepository = rolesRepository;
            _rolesService = rolesService;
        }

        public async Task<OperationResult<long>> CreateAsync(
            RoleCreationModel creationModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            if (authResult.IsFailure)
                return authResult.Error.ToError<long>();

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _rolesService.CreateAsync(creationModel, performedById, cancellationToken);
        }

        public async Task<OperationResult> DeleteAsync(
            long roleId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _rolesService.DeleteAsync(roleId, performedById, cancellationToken);
        }

        public async Task<OperationResult<RoleDetailViewModel>> GetDetailAsync(
            long roleId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            return authResult.IsFailure
                ? authResult.Error.ToError<RoleDetailViewModel>()
                : await _rolesRepository.ReadDetailAsync(
                    roleId: roleId,
                    isDeleted: false,
                    cancellationToken: cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<RoleIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            return authResult.IsFailure
                ? authResult.Error.ToError<IReadOnlyCollection<RoleIdentityViewModel>>()
                : (await _rolesService.GetCurrentIdentitiesAsync(cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult> UpdateAsync(
            long roleId,
            RoleUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)AdministrationPermission.ManageRoles);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _rolesService.UpdateAsync(roleId, updateModel, performedById, cancellationToken);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IRolesRepository _rolesRepository;
        private readonly IRolesService _rolesService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<IRolesOperations, RolesOperations>();
    }
}
