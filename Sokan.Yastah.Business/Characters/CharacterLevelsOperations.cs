using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

    public class CharacterLevelsOperations
        : ICharacterLevelsOperations
    {
        public CharacterLevelsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterLevelsService characterLevelsService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterLevelsService = characterLevelsService;
        }

        public async Task<OperationResult<IReadOnlyList<CharacterLevelDefinitionViewModel>>> GetDefinitionsAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageLevels);

            return authResult.IsFailure
                ? authResult.Error
                : (await _characterLevelsService.GetCurrentDefinitionsAsync(cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageLevels);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterLevelsService.UpdateExperienceDiffsAsync(
                experienceDiffs,
                performedById,
                cancellationToken);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterLevelsService _characterLevelsService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<ICharacterLevelsOperations, CharacterLevelsOperations>();
    }
}
