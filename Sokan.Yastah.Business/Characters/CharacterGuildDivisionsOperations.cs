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
    public interface ICharacterGuildDivisionsOperations
    {
        Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>> GetIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterGuildDivisionsOperations
        : ICharacterGuildDivisionsOperations
    {
        public CharacterGuildDivisionsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterGuildDivisionsService characterGuildDivisionsService,
            ILogger<CharacterGuildDivisionsOperations> logger)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterGuildDivisionsService = characterGuildDivisionsService;
            _logger = logger;
        }

        public async Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageGuilds },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _characterGuildDivisionsService.CreateAsync(guildId, creationModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageGuilds },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _characterGuildDivisionsService.DeleteAsync(guildId, divisionId, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>> GetIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageGuilds },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult.Error;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var result = (await _characterGuildDivisionsService.GetCurrentIdentitiesAsync(guildId, cancellationToken))
                .ToSuccess();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            using var logScope = OperationLogMessages.BeginOperationScope(_logger);
            OperationLogMessages.OperationPerforming(_logger);

            OperationLogMessages.OperationAuthorizing(_logger);
            var authResult = await _authorizationService.RequirePermissionsAsync(
                new[] { (int)CharacterAdministrationPermission.ManageGuilds },
                cancellationToken);

            if (authResult.IsFailure)
            {
                OperationLogMessages.OperationNotAuthorized(_logger);
                return authResult;
            }
            OperationLogMessages.OperationAuthorized(_logger);

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _characterGuildDivisionsService.UpdateAsync(guildId, divisionId, updateModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterGuildDivisionsService _characterGuildDivisionsService;
        private readonly ILogger _logger;
    }
}
