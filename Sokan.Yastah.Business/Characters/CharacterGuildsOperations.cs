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

        Task<OperationResult<long>> CreateDivisionAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteDivisionAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>> GetDivisionIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> GetDivisionIdentityAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken);

        Task<OperationResult<IReadOnlyCollection<CharacterGuildIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult<CharacterGuildIdentityViewModel>> GetIdentityAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateDivisionAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
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
                return authResult.Error.ToError<long>();

            var performedById = _authenticationService.CurrentTicket!.UserId;

            return await _characterGuildsService.CreateAsync(creationModel, performedById, cancellationToken);
        }

        public async Task<OperationResult<long>> CreateDivisionAsync(
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

            return await _characterGuildsService.CreateDivisionAsync(guildId, creationModel, performedById, cancellationToken);
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

        public async Task<OperationResult> DeleteDivisionAsync(
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

            return await _characterGuildsService.DeleteDivisionAsync(guildId, divisionId, performedById, cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>> GetDivisionIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error.ToError<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>>()
                : (await _characterGuildsService.GetCurrentDivisionIdentitiesAsync(guildId, cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> GetDivisionIdentityAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error.ToError<CharacterGuildDivisionIdentityViewModel>()
                : await _characterGuildsService.GetCurrentDivisionIdentityAsync(guildId, divisionId, cancellationToken);
        }

        public async Task<OperationResult<IReadOnlyCollection<CharacterGuildIdentityViewModel>>> GetIdentitiesAsync(
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error.ToError<IReadOnlyCollection<CharacterGuildIdentityViewModel>>()
                : (await _characterGuildsService.GetCurrentIdentitiesAsync(cancellationToken))
                    .ToSuccess();
        }

        public async Task<OperationResult<CharacterGuildIdentityViewModel>> GetIdentityAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            var authResult = await _authorizationService.RequirePermissionsAsync(
                cancellationToken,
                (int)CharacterAdministrationPermission.ManageGuilds);

            return authResult.IsFailure
                ? authResult.Error.ToError<CharacterGuildIdentityViewModel>()
                : await _characterGuildsService.GetCurrentIdentityAsync(guildId, cancellationToken);
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

        public async Task<OperationResult> UpdateDivisionAsync(
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

            return await _characterGuildsService.UpdateDivisionAsync(guildId, divisionId, updateModel, performedById, cancellationToken);
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
