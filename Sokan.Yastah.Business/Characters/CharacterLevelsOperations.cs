using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    public interface ICharacterLevelsOperations
    {
        Task<OperationResult<IReadOnlyList<CharacterLevelDefinitionViewModel>>> GetDefinitionsAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterLevelsOperations
        : ICharacterLevelsOperations
    {
        public CharacterLevelsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterLevelsService characterLevelsService,
            ILogger<CharacterLevelsOperations> logger)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterLevelsService = characterLevelsService;
            _logger = logger;
        }

        public async Task<OperationResult<IReadOnlyList<CharacterLevelDefinitionViewModel>>> GetDefinitionsAsync(
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageLevels },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var result = (await _characterLevelsService.GetCurrentDefinitionsAsync(cancellationToken))
                .ToSuccess();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageLevels },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _characterLevelsService.UpdateExperienceDiffsAsync(
                experienceDiffs,
                performedById,
                cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterLevelsService _characterLevelsService;
        private readonly ILogger _logger;
    }
}
