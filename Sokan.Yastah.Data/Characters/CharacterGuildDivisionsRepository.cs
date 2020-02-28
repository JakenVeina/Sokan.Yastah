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
    public interface ICharacterGuildDivisionsRepository
    {
        Task<bool> AnyVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedDivisionIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<CharacterGuildDivisionIdentityViewModel> AsyncEnumerateIdentities(
            Optional<long> guildId = default,
            Optional<bool> isDeleted = default);

        Task<long> CreateAsync(
            long guildId,
            string name,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateAsync(
            long divisionId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterGuildDivisionsRepository
        : ICharacterGuildDivisionsRepository
    {
        public CharacterGuildDivisionsRepository(
            YastahDbContext context,
            ILogger<CharacterGuildDivisionsRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedDivisionIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildDivisionVersionsEnumeratingAny(_logger, guildId, excludedDivisionIds, name, isDeleted, isLatestVersion);

            var query = _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (guildId.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(guildId));
                query = query.Where(x => x.Division.GuildId == guildId.Value);
            }

            if (excludedDivisionIds.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(excludedDivisionIds));
                query = query.Where(x => !excludedDivisionIds.Value.Contains(x.DivisionId));
            }

            if (name.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(name));
                query = query.Where(x => x.Name == name.Value);
            }

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            if (isLatestVersion.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isLatestVersion));
                query = isLatestVersion.Value
                    ? query.Where(x => x.NextVersionId == null)
                    : query.Where(x => x.NextVersionId != null);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query.AnyAsync(cancellationToken);

            RepositoryLogMessages.QueryExecuting(_logger);
            return result;
        }

        public IAsyncEnumerable<CharacterGuildDivisionIdentityViewModel> AsyncEnumerateIdentities(
                Optional<long> guildId = default,
                Optional<bool> isDeleted = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildDivisionIdentitiesEnumerating(_logger, guildId, isDeleted);

            var query = _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (guildId.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(guildId));
                query = query.Where(x => x.Division.GuildId == guildId.Value);
            }

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(CharacterGuildDivisionIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<long> CreateAsync(
            long guildId,
            string name,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildDivisionCreating(_logger, guildId, name, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var division = new CharacterGuildDivisionEntity(
                id:         default,
                guildId:    guildId);
            await _context.AddAsync(division, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterGuildDivisionVersionEntity(
                id:                 default,
                divisionId:         division.Id,
                name:               name,
                isDeleted:          false,
                creationId:         actionId,
                previousVersionId:  null,
                nextVersionId:      null);
            await _context.AddAsync(version, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            CharactersLogMessages.CharacterGuildDivisionCreated(_logger, division.Id, version.Id);
            return division.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long divisionId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildDivisionUpdating(_logger, divisionId, actionId, name, isDeleted);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var currentVersion = await _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable()
                .Where(x => x.DivisionId == divisionId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
            {
                CharactersLogMessages.CharacterGuildDivisionVersionNotFound(_logger, divisionId);
                return new DataNotFoundError($"Character Guild Division ID {divisionId}");
            }

            var newVersion = new CharacterGuildDivisionVersionEntity(
                id:                 default,
                divisionId:         currentVersion.DivisionId,
                name:               name.IsSpecified
                                        ? name.Value
                                        : currentVersion.Name,
                isDeleted:          isDeleted.IsSpecified
                                        ? isDeleted.Value
                                        : currentVersion.IsDeleted,
                creationId:         actionId,
                previousVersionId:  currentVersion.Id,
                nextVersionId:      null);

            if ((newVersion.Name == currentVersion.Name)
                && (newVersion.IsDeleted == currentVersion.IsDeleted))
            {
                TransactionsLogMessages.TransactionScopeCommitting(_logger);
                transactionScope.Complete();

                CharactersLogMessages.CharacterGuildDivisionNoChangesGiven(_logger, divisionId);
                return new NoChangesGivenError($"Character Guild Division ID {divisionId}");
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterGuildDivisionUpdated(_logger, divisionId, newVersion.Id);
            return newVersion.Id
                .ToSuccess();
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
