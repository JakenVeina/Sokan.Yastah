using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Administration;
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
            ILogger<CharacterGuildDivisionsService> logger,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _characterGuildsRepository = characterGuildsRepository;
            _characterGuildDivisionsRepository = characterGuildDivisionsRepository;
            _logger = logger;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<OperationResult<long>> CreateAsync(
            long guildId,
            CharacterGuildDivisionCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildDivisionCreating(_logger, guildId, creationModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildIdValidationFailed(_logger, guildId, guildIdValidationResult);
                return guildIdValidationResult.Error;
            }
            CharactersLogMessages.CharacterGuildIdValidationSucceeded(_logger, guildId);

            var nameValidationResult = await ValidateDivisionNameAsync(guildId, creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildDivisionNameValidationFailed(_logger, creationModel.Name, nameValidationResult);
                return nameValidationResult.Error;
            }
            CharactersLogMessages.CharacterGuildDivisionNameValidationSucceeded(_logger, creationModel.Name);

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var divisionId = await _characterGuildDivisionsRepository.CreateAsync(
                guildId,
                creationModel.Name,
                actionId,
                cancellationToken);
            CharactersLogMessages.CharacterGuildDivisionCreated(_logger, guildId, divisionId);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            return divisionId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            long divisionId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildDivisionDeleting(_logger, guildId, divisionId, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildIdValidationFailed(_logger, guildId, guildIdValidationResult);
                return guildIdValidationResult;
            }
            CharactersLogMessages.CharacterGuildIdValidationSucceeded(_logger, guildId);

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var updateResult = await _characterGuildDivisionsRepository.UpdateAsync(
                divisionId: divisionId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (updateResult.IsSuccess)
            {
                CharactersLogMessages.CharacterGuildDivisionDeleted(_logger, guildId, divisionId);
                transactionScope.Complete();
                TransactionsLogMessages.TransactionScopeCommitted(_logger);
            }
            else
                CharactersLogMessages.CharacterGuildDivisionDeleteFailed(_logger, guildId, divisionId, updateResult);

            return updateResult;
        }

        public async Task<IReadOnlyCollection<CharacterGuildDivisionIdentityViewModel>> GetCurrentIdentitiesAsync(
                long guildId,
                CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildDivisionIdentitiesFetchingCurrent(_logger, guildId);

            var identities = await _characterGuildDivisionsRepository.AsyncEnumerateIdentities(
                    guildId: guildId,
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);
            CharactersLogMessages.CharacterGuildDivisionIdentitiesFetchedCurrent(_logger, guildId);

            return identities;
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            long divisionId,
            CharacterGuildDivisionUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildDivisionUpdating(_logger, guildId, divisionId, updateModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var guildIdValidationResult = await ValidateGuildIdAsync(guildId, cancellationToken);
            if (guildIdValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildIdValidationFailed(_logger, guildId, guildIdValidationResult);
                return guildIdValidationResult;
            }
            CharactersLogMessages.CharacterGuildIdValidationSucceeded(_logger, guildId);

            var nameValidationResult = await ValidateDivisionNameAsync(guildId, updateModel.Name, divisionId, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildDivisionNameValidationFailed(_logger, updateModel.Name, nameValidationResult);
                return nameValidationResult;
            }
            CharactersLogMessages.CharacterGuildDivisionNameValidationSucceeded(_logger, updateModel.Name);

            var now = _systemClock.UtcNow;

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.DivisionModified,
                now,
                performedById,
                cancellationToken);
            AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

            var updateResult = await _characterGuildDivisionsRepository.UpdateAsync(
                divisionId: divisionId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);

            if (updateResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildDivisionUpdateFailed(_logger, guildId, divisionId, updateResult);
                return updateResult;
            }
            CharactersLogMessages.CharacterGuildDivisionUpdated(_logger, guildId, divisionId);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

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
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
