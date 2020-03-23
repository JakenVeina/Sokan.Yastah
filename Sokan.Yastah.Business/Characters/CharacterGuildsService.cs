using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Auditing;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Auditing;
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
            IAuditableActionsRepository auditableActionsRepository,
            ICharacterGuildsRepository characterGuildsRepository,
            ILogger<CharacterGuildsService> logger,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _auditableActionsRepository = auditableActionsRepository;
            _characterGuildsRepository = characterGuildsRepository;
            _logger = logger;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<OperationResult<long>> CreateAsync(
            CharacterGuildCreationModel creationModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildCreating(_logger, creationModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var nameValidationResult = await ValidateNameAsync(creationModel.Name, null, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildNameValidationFailed(_logger, creationModel.Name, nameValidationResult);
                return nameValidationResult.Error;
            }
            CharactersLogMessages.CharacterGuildNameValidationSucceeded(_logger, creationModel.Name);

            var actionId = await _auditableActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildCreated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AuditingLogMessages.AuditingActionCreated(_logger, actionId);

            var guildId = await _characterGuildsRepository.CreateAsync(
                creationModel.Name,
                actionId,
                cancellationToken);
            CharactersLogMessages.CharacterGuildCreated(_logger, guildId);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            return guildId.ToSuccess();
        }

        public async Task<OperationResult> DeleteAsync(
            long guildId,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildDeleting(_logger, guildId, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var actionId = await _auditableActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildDeleted,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AuditingLogMessages.AuditingActionCreated(_logger, actionId);

            var updateResult = await _characterGuildsRepository.UpdateAsync(
                guildId: guildId,
                actionId: actionId,
                isDeleted: true,
                cancellationToken: cancellationToken);

            if (updateResult.IsSuccess)
            {
                CharactersLogMessages.CharacterGuildDeleted(_logger, guildId);
                transactionScope.Complete();
                TransactionsLogMessages.TransactionScopeCommitted(_logger);
            }
            else
                CharactersLogMessages.CharacterGuildDeleteFailed(_logger, guildId, updateResult);

            return updateResult;
        }

        public async Task<IReadOnlyCollection<CharacterGuildIdentityViewModel>> GetCurrentIdentitiesAsync(
                CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildIdentitiesFetchingCurrent(_logger);

            var identities = await _characterGuildsRepository.AsyncEnumerateIdentities(
                    isDeleted: false)
                .ToArrayAsync(cancellationToken);
            CharactersLogMessages.CharacterGuildIdentitiesFetchedCurrent(_logger);

            return identities;
        }

        public async Task<OperationResult> UpdateAsync(
            long guildId,
            CharacterGuildUpdateModel updateModel,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterGuildUpdating(_logger, guildId, updateModel, performedById);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var nameValidationResult = await ValidateNameAsync(updateModel.Name, guildId, cancellationToken);
            if (nameValidationResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildNameValidationFailed(_logger, updateModel.Name, nameValidationResult);
                return nameValidationResult;
            }
            CharactersLogMessages.CharacterGuildNameValidationSucceeded(_logger, updateModel.Name);

            var now = _systemClock.UtcNow;

            var actionId = await _auditableActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.GuildModified,
                now,
                performedById,
                cancellationToken);
            AuditingLogMessages.AuditingActionCreated(_logger, actionId);

            var updateResult = await _characterGuildsRepository.UpdateAsync(
                guildId: guildId,
                actionId: actionId,
                name: updateModel.Name,
                cancellationToken: cancellationToken);
            
            if (updateResult.IsFailure)
            {
                CharactersLogMessages.CharacterGuildUpdateFailed(_logger, guildId, updateResult);
                return updateResult;
            }
            CharactersLogMessages.CharacterGuildUpdated(_logger, guildId);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

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

        private readonly IAuditableActionsRepository _auditableActionsRepository;
        private readonly ICharacterGuildsRepository _characterGuildsRepository;
        private readonly ILogger _logger;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
