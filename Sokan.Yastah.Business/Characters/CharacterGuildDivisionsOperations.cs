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

    public class CharacterGuildDivisionsOperations
        : ICharacterGuildDivisionsOperations
    {
        public CharacterGuildDivisionsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterGuildDivisionsService characterGuildDivisionsService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterGuildDivisionsService = characterGuildDivisionsService;
        }

        public async Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            if (authResult.IsFailure)
                return authResult.Error.ToError<long>();

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildDivisionsService.CreateAsync(guildId, creationModel, performedById, cancellationToken);
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildDivisionsService.DeleteAsync(guildId, divisionId, performedById, cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>> GetIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error.ToError<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>()
                : (await _characterGuildDivisionsService.GetCurrentIdentitiesAsync(guildId, cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);
            
            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildDivisionsService.UpdateAsync(guildId, divisionId, updateModel, performedById, cancellationToken);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterGuildDivisionsService _characterGuildDivisionsService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<ICharacterGuildDivisionsOperations, CharacterGuildDivisionsOperations>();
    }
}
