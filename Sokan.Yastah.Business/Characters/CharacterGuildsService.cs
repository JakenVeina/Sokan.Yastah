using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.OperationModel;
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

        Task<OperationResult> DeleteAsync(
            long guildId,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<CharacterGuildIdentityViewModel>> GetCurrentIdentitiesAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
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
                return nameValidationResult.Error;

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

        public async Task<IReadOnlyCollection<CharacterGuildIdentityViewModel>> GetCurrentIdentitiesAsync(
                CancellationToken cancellationToken)
            => await _characterGuildsRepository.AsyncEnumerateIdentities(
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);

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
                ? new NameInUseError(name)
                : OperationResult.Success;
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly ICharacterGuildsRepository _characterGuildsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
