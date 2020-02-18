using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            long creationId,
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
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
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
            var query = _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable();

            if (guildId.IsSpecified)
                query = query.Where(x => x.GuildId == guildId.Value);

            if (excludedGuildIds.IsSpecified)
                query = query.Where(x => !excludedGuildIds.Value.Contains(x.GuildId));

            if (name.IsSpecified)
                query = query.Where(x => x.Name == name.Value);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            if (isLatestVersion.IsSpecified)
                query = isLatestVersion.Value
                    ? query.Where(x => x.NextVersionId == null)
                    : query.Where(x => x.NextVersionId != null);

            return query.AnyAsync(cancellationToken);
        }

        public IAsyncEnumerable<CharacterGuildIdentityViewModel> AsyncEnumerateIdentities(
                Optional<bool> isDeleted = default)
        {
            var query = _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            return query
                .Select(CharacterGuildIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();
        }

        public async Task<long> CreateAsync(
            string name,
            long creationId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var guild = new CharacterGuildEntity(
                id: default);

            await _context.AddAsync(guild, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterGuildVersionEntity(
                id:                 default,
                guildId:            guild.Id,
                name:               name,
                isDeleted:          false,
                creationId:         creationId,
                previousVersionId:  null,
                nextVersionId:      null);

            await _context.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return guild.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long guildId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var currentVersion = await _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable()
                .Where(x => x.GuildId == guildId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
                return new DataNotFoundError($"Character Guild ID {guildId}");

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
                transactionScope.Complete();
                return new NoChangesGivenError($"Character Guild ID {guildId}");
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return newVersion.Id
                .ToSuccess();
        }

        private readonly YastahDbContext _context;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
