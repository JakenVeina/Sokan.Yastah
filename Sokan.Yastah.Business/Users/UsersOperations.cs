using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Business.Users
{
    public interface IUsersOperations
    {
        Task<OperationResult<UserDetailViewModel>> GetDetailAsync(
            ulong userId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<UserOverviewViewModel>>> GetOverviewsAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class UsersOperations
        : IUsersOperations
    {
        public UsersOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ILogger<UsersOperations> logger,
            IUsersRepository usersRepository,
            IUsersService usersService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _logger = logger;
            _usersRepository = usersRepository;
            _usersService = usersService;
        }

        public async Task<OperationResult<UserDetailViewModel>> GetDetailAsync(
            ulong userId,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
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

            var result = await _usersRepository.ReadDetailAsync(userId, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult<IReadOnlyCollection<UserOverviewViewModel>>> GetOverviewsAsync(
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
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

            var result = (await _usersRepository.AsyncEnumerateOverviews()
                    .ToArrayAsync(cancellationToken))
                .ToSuccess<IReadOnlyCollection<UserOverviewViewModel>>();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }
        public async Task<OperationResult> UpdateAsync(
            ulong userId,
            UserUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
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

            var result = await _usersService.UpdateAsync(userId, updateModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger _logger;
        private readonly IUsersRepository _usersRepository;
        private readonly IUsersService _usersService;
    }
}
