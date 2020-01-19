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

    public class CharacterGuildsOperations
        : ICharacterGuildsOperations
    {
        public CharacterGuildsOperations(
            IAuthenticationService authenticationService,
            IAuthorizationService authorizationService,
            ICharacterGuildsService characterGuildsService)
        {
            _authenticationService = authenticationService;
            _authorizationService = authorizationService;
            _characterGuildsService = characterGuildsService;
        }

        public async Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            if (authResult.IsFailure)
                return authResult.Error;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildsService.CreateAsync(creationModel, performedById, cancellationToken);
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildsService.DeleteAsync(guildId, performedById, cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error
                : (await _characterGuildsService.GetCurrentIdentitiesAsync(cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            if (authResult.IsFailure)
                return authResult;

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildsService.UpdateAsync(guildId, updateModel, performedById, cancellationToken);
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICharacterGuildsService _characterGuildsService;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<ICharacterGuildsOperations, CharacterGuildsOperations>();
    }
}
