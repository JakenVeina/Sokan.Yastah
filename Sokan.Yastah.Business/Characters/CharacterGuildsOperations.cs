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
    public interface ICharacterGuildsOperations
    {
        Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<CharacterGuildIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterGuildsOperations
        : ICharacterGuildsOperations
    {
        public CharacterGuildsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterGuildsService characterGuildsService,
            ILogger<CharacterGuildsOperations> logger)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterGuildsService = characterGuildsService;
            _logger = logger;
        }

        public async Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
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

            var result = await _characterGuildsService.CreateAsync(creationModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> DeleteAsync(
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

            var performedById = _authenticationService.CurrentTicket!.UserId;

            var result = await _characterGuildsService.DeleteAsync(guildId, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildIdentityViewModel>>> GetIdentitiesAsync(
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

            var result = (await _characterGuildsService.GetCurrentIdentitiesAsync(cancellationToken)).ToSuccess();
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
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

            var result = await _characterGuildsService.UpdateAsync(guildId, updateModel, performedById, cancellationToken);
            OperationLogMessages.OperationPerformed(_logger, result);

            return result;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterGuildsService _characterGuildsService;
        private readonly ILogger _logger;
    }
}
