using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    public interface ICharacterGuildsService
    {
        Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> CreateDivisionAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long guildId,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteDivisionAsync(
            long guildId,
            long divisionId,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>> GetCurrentDivisionIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> GetCurrentDivisionIdentityAsync(
            long guildId,
            long divisionId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<CharacterGuildIdentityViewModel>> GetCurrentIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult<CharacterGuildIdentityViewModel>> GetCurrentIdentityAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateDivisionAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    public class CharacterGuildsService
        : ICharacterGuildsService
    {
        public CharacterGuildsService(
            IAdministrationActionsRepository administrationActionsRepository,
            ICharacterGuildsRepository characterGuildsRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _characterGuildsRepository = characterGuildsRepository;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var nameValidationResult = await ValidateNameAsync(creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult.Error.ToError<long>();

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var guildId = await _characterGuildsRepository.CreateAsync(
                creationModel.Name,
                actionId,
                cancellationToken);

            transactionScope.Complete();

            return guildId.ToSuccess();
        }

        public async Task<OperationResult<long>> CreateDivisionAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
                return guildIdValidationResult.Error.ToError<long>();

            var nameValidationResult = await ValidateDivisionNameAsync(guildId, creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult.Error.ToError<long>();

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var divisionId = await _characterGuildsRepository.CreateDivisionAsync(
                guildId,
                creationModel.Name,
                actionId,
                cancellationToken);

            transactionScope.Complete();

            return divisionId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var updateResult = await _characterGuildsRepository.UpdateAsync(
                guildId: guildId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (updateResult.IsSuccess)
                transactionScope.Complete();

            return updateResult;
        }

        public async Task<OperationResult> DeleteDivisionAsync(
            long guildId,
            long divisionId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
                return guildIdValidationResult;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var updateResult = await _characterGuildsRepository.UpdateDivisionAsync(
                divisionId: divisionId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (updateResult.IsSuccess)
                transactionScope.Complete();

            return updateResult;
        }

        public async Task<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>> GetCurrentDivisionIdentitiesAsync(
                long guildId,
                CancellationToken cancellationToken)
            => await _characterGuildsRepository.AsyncEnumerateDivisionIdentities(
                    guildId: guildId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);

        public async Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> GetCurrentDivisionIdentityAsync(
                long guildId,
                long divisionId,
                CancellationToken cancellationToken)
            => await _characterGuildsRepository.ReadDivisionIdentityAsync(
                guildId: guildId,
                divisionId: divisionId,
                isDeleted: false,
                cancellationToken: cancellationToken);

        public async Task<IReadOnlyCollection<CharacterGuildIdentityViewModel>> GetCurrentIdentitiesAsync(
                CancellationToken cancellationToken)
            => await _characterGuildsRepository.AsyncEnumerateIdentities(
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);

        public async Task<OperationResult<CharacterGuildIdentityViewModel>> GetCurrentIdentityAsync(
                long guildId,
                CancellationToken cancellationToken)
            => await _characterGuildsRepository.ReadIdentityAsync(
                guildId: guildId,
                isDeleted: false,
                cancellationToken: cancellationToken);

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var nameValidationResult = await ValidateNameAsync(updateModel.Name, guildId, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult;

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildModified,
                now,
                performedById,
                cancellationToken);

            var updateResult = await _characterGuildsRepository.UpdateAsync(
                guildId: guildId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);
            if (updateResult.IsFailure)
                return updateResult;

            transactionScope.Complete();

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateDivisionAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
                return guildIdValidationResult;

            var nameValidationResult = await ValidateDivisionNameAsync(guildId, updateModel.Name, divisionId, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult;

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionModified,
                now,
                performedById,
                cancellationToken);

            var updateResult = await _characterGuildsRepository.UpdateDivisionAsync(
                divisionId: divisionId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);
            if (updateResult.IsFailure)
                return updateResult;

            transactionScope.Complete();

            return OperationResult.Success;
        }

        private async Task<OperationResult> ValidateGuildIdAsync(
            long guildId,
            CancellationToken cancellationToken)
        {
            var guildIdIsActive = await _characterGuildsRepository.AnyVersionsAsync(
                guildId: guildId,
                isDeleted: false,
                isLatestVersion: true,
                cancellationToken: cancellationToken);

            return guildIdIsActive
                ? OperationResult.Success
                : new DataNotFoundError($"Guild ID {guildId}").ToError();
        }

        private async Task<OperationResult> ValidateDivisionNameAsync(
            long guildId,
            string name,
            long? divisionId,
            CancellationToken cancellationToken)
        {
            var nameIsInUse = await _characterGuildsRepository.AnyDivisionVersionsAsync(
                guildId: guildId,
                excludedDivisionIds: divisionId?.ToEnumerable()?.ToOptional() ?? default,
                name: name,
                isDeleted: false,
                isLatestVersion: true,
                cancellationToken: cancellationToken);

            return nameIsInUse
                ? new NameInUseError(name).ToError()
                : OperationResult.Success;
        }

        private async Task<OperationResult> ValidateNameAsync(
            string name,
            long? guildId,
            CancellationToken cancellationToken)
        {
            var nameIsInUse = await _characterGuildsRepository.AnyVersionsAsync(
                excludedGuildIds: guildId?.ToEnumerable()?.ToOptional() ?? default,
                name: name,
                isDeleted: false,
                isLatestVersion: true,
                cancellationToken: cancellationToken);

            return nameIsInUse
                ? new NameInUseError(name).ToError()
                : OperationResult.Success;
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly ICharacterGuildsRepository _characterGuildsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<ICharacterGuildsService, CharacterGuildsService>();
    }
}
