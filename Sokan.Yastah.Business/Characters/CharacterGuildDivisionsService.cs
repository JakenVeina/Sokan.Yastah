using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    public interface ICharacterGuildDivisionsService
    {
        Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<OperationResult> DeleteAsync(
            long guildId,
            long divisionId,
            ulong performedById,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>> GetCurrentIdentitiesAsync(
            long guildId,
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterGuildDivisionsService
        : ICharacterGuildDivisionsService
    {
        public CharacterGuildDivisionsService(
            IAdministrationActionsRepository administrationActionsRepository,
            ICharacterGuildsRepository characterGuildsRepository,
            ICharacterGuildDivisionsRepository characterGuildDivisionsRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _characterGuildsRepository = characterGuildsRepository;
            _characterGuildDivisionsRepository = characterGuildDivisionsRepository;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
                return guildIdValidationResult.Error;

            var nameValidationResult = await ValidateDivisionNameAsync(guildId, creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
                return nameValidationResult.Error;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var divisionId = await _characterGuildDivisionsRepository.CreateAsync(
                guildId,
                creationModel.Name,
                actionId,
                cancellationToken);

            transactionScope.Complete();

            return divisionId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
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

            var updateResult = await _characterGuildDivisionsRepository.UpdateAsync(
                divisionId: divisionId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (updateResult.IsSuccess)
                transactionScope.Complete();

            return updateResult;
        }

        public async Task<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>> GetCurrentIdentitiesAsync(
                long guildId,
                CancellationToken cancellationToken)
            => await _characterGuildDivisionsRepository.AsyncEnumerateIdentities(
                    guildId: guildId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);

        public async Task<OperationResult> UpdateAsync(
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

            var updateResult = await _characterGuildDivisionsRepository.UpdateAsync(
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
                : new DataNotFoundError($"Guild ID {guildId}");
        }

        private async Task<OperationResult> ValidateDivisionNameAsync(
            long guildId,
            string name,
            long? divisionId,
            CancellationToken cancellationToken)
        {
            var nameIsInUse = await _characterGuildDivisionsRepository.AnyVersionsAsync(
                guildId: guildId,
                excludedDivisionIds: divisionId?.ToEnumerable()?.ToOptional() ?? default,
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
        private readonly ICharacterGuildDivisionsRepository _characterGuildDivisionsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
