using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Characters
{
    public interface ICharactersRepository
    {
        Task<long> CreateAsync(
            ulong ownerId,
            string name,
            long divisionId,
            decimal experiencePoints,
            decimal goldAmount,
            decimal insanityValue,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateAsync(
            long characterId,
            long actionId,
            Optional<string> name = default,
            Optional<long> divisionId = default,
            Optional<decimal> experiencePoints = default,
            Optional<decimal> goldAmount = default,
            Optional<decimal> insanityValue = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

    public class CharactersRepository
        : ICharactersRepository
    {
        public CharactersRepository(
            YastahDbContext context,
            ILogger<CharactersRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<long> CreateAsync(
            ulong ownerId,
            string name,
            long divisionId,
            decimal experiencePoints,
            decimal goldAmount,
            decimal insanityValue,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterCreating(_logger, ownerId, name, divisionId, experiencePoints, goldAmount, insanityValue, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var character = new CharacterEntity(
                id:         default,
                ownerId:    ownerId);
            await _context.AddAsync(character, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterVersionEntity(
                id:                 default,
                characterId:        character.Id,
                divisionId:         divisionId,
                name:               name,
                experiencePoints:   experiencePoints,
                goldAmount:         goldAmount,
                insanityValue:      insanityValue,
                isDeleted:          false,
                creationId:         actionId,
                previousVersionId:  null,
                nextVersionId:      null);
            await _context.AddAsync(version, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterCreated(_logger, character.Id, version.Id);
            return character.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long characterId,
            long actionId,
            Optional<string> name = default,
            Optional<long> divisionId = default,
            Optional<decimal> experiencePoints = default,
            Optional<decimal> goldAmount = default,
            Optional<decimal> insanityValue = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterUpdating(_logger, characterId, actionId, name, divisionId, experiencePoints, goldAmount, insanityValue, isDeleted);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var currentVersion = await _context.Set<CharacterVersionEntity>()
                .AsQueryable()
                .Where(x => x.CharacterId == characterId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
            {
                CharactersLogMessages.CharacterVersionNotFound(_logger, characterId);
                return new DataNotFoundError($"Character ID {characterId}");
            }

            var newVersion = new CharacterVersionEntity(
                id:                 default,
                characterId:        currentVersion.CharacterId,
                name:               name.IsSpecified
                                        ? name.Value
                                        : currentVersion.Name,
                divisionId:         divisionId.IsSpecified
                                        ? divisionId.Value
                                        : currentVersion.DivisionId,
                experiencePoints:   experiencePoints.IsSpecified
                                        ? experiencePoints.Value
                                        : currentVersion.ExperiencePoints,
                goldAmount:         goldAmount.IsSpecified
                                        ? goldAmount.Value
                                        : currentVersion.GoldAmount,
                insanityValue:      insanityValue.IsSpecified
                                        ? insanityValue.Value
                                        : currentVersion.InsanityValue,
                isDeleted:          isDeleted.IsSpecified
                                        ? isDeleted.Value
                                        : currentVersion.IsDeleted,
                creationId:         actionId,
                previousVersionId:  currentVersion.Id,
                nextVersionId:      null);

            if ((newVersion.Name == currentVersion.Name)
                && (newVersion.DivisionId == currentVersion.DivisionId)
                && (newVersion.ExperiencePoints == currentVersion.ExperiencePoints)
                && (newVersion.GoldAmount == currentVersion.GoldAmount)
                && (newVersion.InsanityValue == currentVersion.InsanityValue)
                && (newVersion.IsDeleted == currentVersion.IsDeleted))
            {
                TransactionsLogMessages.TransactionScopeCommitting(_logger);
                transactionScope.Complete();

                CharactersLogMessages.CharacterNoChangesGiven(_logger, characterId);
                return new NoChangesGivenError($"Character ID {characterId}");
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterUpdated(_logger, characterId, newVersion.Id);
            return newVersion.Id
                .ToSuccess();
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
