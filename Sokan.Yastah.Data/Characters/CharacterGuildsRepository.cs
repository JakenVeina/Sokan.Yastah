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
    public interface ICharacterGuildsRepository
    {
        Task<bool> AnyVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedGuildIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<CharacterGuildIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default);

        Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateAsync(
            long guildId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterGuildsRepository
        : ICharacterGuildsRepository
    {
        public CharacterGuildsRepository(
            YastahDbContext context,
            ILogger<CharacterGuildsRepository> logger,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _logger = logger;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedGuildIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildVersionsEnumeratingAny(_logger, guildId, excludedGuildIds, name, isDeleted, isLatestVersion);

            var query = _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable();
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (guildId.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(guildId));
                query = query.Where(x => x.GuildId == guildId.Value);
            }

            if (excludedGuildIds.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(excludedGuildIds));
                query = query.Where(x => !excludedGuildIds.Value.Contains(x.GuildId));
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

        public IAsyncEnumerable<CharacterGuildIdentityViewModel> AsyncEnumerateIdentities(
                Optional<bool> isDeleted = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildIdentitiesEnumerating(_logger, isDeleted);

            var query = _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);
            RepositoryLogMessages.QueryInitializing(_logger, query);

            if (isDeleted.IsSpecified)
            {
                RepositoryLogMessages.QueryAddingWhereClause(_logger, nameof(isDeleted));
                query = query.Where(x => x.IsDeleted == isDeleted.Value);
            }

            RepositoryLogMessages.QueryTerminating(_logger);
            var result = query
                .Select(CharacterGuildIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();

            RepositoryLogMessages.QueryBuilt(_logger);
            return result;
        }

        public async Task<long> CreateAsync(
            string name,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildCreating(_logger, name, actionId);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var guild = new CharacterGuildEntity(
                id: default);
            await _context.AddAsync(guild, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterGuildVersionEntity(
                id:                 default,
                guildId:            guild.Id,
                name:               name,
                isDeleted:          false,
                creationId:         actionId,
                previousVersionId:  null,
                nextVersionId:      null);
            await _context.AddAsync(version, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterGuildCreated(_logger, guild.Id, version.Id);
            return guild.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long guildId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var logScope = _logger.BeginMemberScope();
            CharactersLogMessages.CharacterGuildUpdating(_logger, guildId, actionId, name, isDeleted);

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var currentVersion = await _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable()
                .Where(x => x.GuildId == guildId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
            {
                CharactersLogMessages.CharacterGuildVersionNotFound(_logger, guildId);
                return new DataNotFoundError($"Character Guild ID {guildId}");
            }

            var newVersion = new CharacterGuildVersionEntity(
                id:                 default,
                guildId:            currentVersion.GuildId,
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

                CharactersLogMessages.CharacterGuildNoChangesGiven(_logger, guildId);
                return new NoChangesGivenError($"Character Guild ID {guildId}");
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);

            YastahDbContextLogMessages.ContextSavingChanges(_logger);
            await _context.SaveChangesAsync(cancellationToken);

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterGuildUpdated(_logger, guildId, newVersion.Id);
            return newVersion.Id
                .ToSuccess();
        }

        private readonly YastahDbContext _context;
        private readonly ILogger _logger;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
