using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Characters
{
    public interface ICharacterLevelsRepository
    {
        Task<bool> AnyDefinitionsAsync(
            Optional<int> level = default,
            Optional<int> experienceThreshold = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<CharacterLevelDefinitionViewModel> AsyncEnumerateDefinitions(
            Optional<bool> isDeleted = default);

        Task<OperationResult> MergeDefinitionAsync(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId,
            CancellationToken cancellationToken);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterLevelsRepository
        : ICharacterLevelsRepository
    {
        public CharacterLevelsRepository(
            YastahDbContext context,
            ILogger<CharacterLevelsRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyDefinitionsAsync(
            Optional<int> level = default,
            Optional<int> experienceThreshold = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterLevelDefinitionsEnumeratingAny(_logger, level, experienceThreshold, isDeleted);

            var query = _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (level.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(level));
                query = query.Where(cld => cld.Level == level.Value);
            }

            if (experienceThreshold.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(experienceThreshold));
                query = query.Where(cld => cld.ExperienceThreshold == experienceThreshold.Value);
            }

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(cld => cld.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query.AnyAsync(cancellationToken);

            RepositoryLogMessages.QueryExecuting(_logger);
            return result;
        }

        public IAsyncEnumerable<CharacterLevelDefinitionViewModel> AsyncEnumerateDefinitions(
            Optional<bool> isDeleted = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterLevelDefinitionsEnumerating(_logger, isDeleted);

            var query = _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(cld => cld.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .OrderBy(cld => cld.Level)
                .Select(CharacterLevelDefinitionViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<OperationResult> MergeDefinitionAsync(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterLevelDefinitionMerging(_logger, level, experienceThreshold, isDeleted, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var currentVersion = await _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.Level == level)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            var newVersion = new CharacterLevelDefinitionVersionEntity(
                id:                     default,
                level:                  level,
                experienceThreshold:    experienceThreshold,
                isDeleted:              isDeleted,
                creationId:             actionId,
                previousVersionId:      null,
                nextVersionId:          null);

            if (currentVersion is null)
            {
                CharactersLogMessages.CharacterLevelDefinitionCreating(_logger, level);

                var definition = new CharacterLevelDefinitionEntity(
                    level: level);
                await _context.AddAsync(definition, cancellationToken);

                YastahDbContextLogMessages.ContextSavingChanges(_logger);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                CharactersLogMessages.CharacterLevelDefinitionVersionCreating(_logger, level);
                
                if ((newVersion.ExperienceThreshold == currentVersion.ExperienceThreshold)
                    && (newVersion.IsDeleted == currentVersion.IsDeleted))
                {
                    TransactionsLogMessages.TransactionScopeCommitting(_logger);
                    transactionScope.Complete();

                    CharactersLogMessages.CharacterLevelDefinitionNoChangesGiven(_logger, level, currentVersion.Id);
                    return new NoChangesGivenError($"Character Level Definition {level}");
                }

                newVersion.PreviousVersionId = currentVersion.Id;
                currentVersion.NextVersion = newVersion;
            }

            await _context.AddAsync(newVersion, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterLevelDefinitionMerged(_logger, level, newVersion.Id);
            return OperationResult.Success;
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
