﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Characters
{
    public interface ICharacterGuildsRepository
    {
        Task<bool> AnyDivisionVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedDivisionIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default);

        Task<bool> AnyVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedGuildIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<CharacterGuildDivisionIdentityViewModel> AsyncEnumerateDivisionIdentities(
            Optional<long> guildId = default,
            Optional<bool> isDeleted = default);

        IAsyncEnumerable<CharacterGuildIdentityViewModel> AsyncEnumerateIdentities(
            Optional<bool> isDeleted = default);

        Task<long> CreateAsync(
            string name,
            long creationId,
            CancellationToken cancellationToken);

        Task<long> CreateDivisionAsync(
            long guildId,
            string name,
            long creationId,
            CancellationToken cancellationToken);

        Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> ReadDivisionIdentityAsync(
            long divisionId,
            Optional<long> guildId = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        Task<OperationResult<CharacterGuildIdentityViewModel>> ReadIdentityAsync(
            long guildId,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        Task<OperationResult<long>> UpdateAsync(
            long guildId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);

        Task<OperationResult<long>> UpdateDivisionAsync(
            long divisionId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

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

        public Task<bool> AnyDivisionVersionsAsync(
            Optional<long> guildId = default,
            Optional<IEnumerable<long>> excludedDivisionIds = default,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            Optional<bool> isLatestVersion = default,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable();

            if (guildId.IsSpecified)
                query = query.Where(x => x.Division.GuildId == guildId.Value);

            if (excludedDivisionIds.IsSpecified)
                query = query.Where(x => !excludedDivisionIds.Value.Contains(x.DivisionId));

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

        public IAsyncEnumerable<CharacterGuildDivisionIdentityViewModel> AsyncEnumerateDivisionIdentities(
                Optional<long> guildId = default,
                Optional<bool> isDeleted = default)
        {
            var query = _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);

            if (guildId.IsSpecified)
                query = query.Where(x => x.Division.GuildId == guildId.Value);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            return query
                .Select(CharacterGuildDivisionIdentityViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();
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

        public async Task<long> CreateDivisionAsync(
            long guildId,
            string name,
            long creationId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var division = new CharacterGuildDivisionEntity(
                id:         default,
                guildId:    guildId);

            await _context.AddAsync(division, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterGuildDivisionVersionEntity(
                id:                 default,
                divisionId:         division.Id,
                name:               name,
                isDeleted:          false,
                creationId:         creationId,
                previousVersionId:  null,
                nextVersionId:      null);

            await _context.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return division.Id;
        }

        public async Task<OperationResult<CharacterGuildDivisionIdentityViewModel>> ReadDivisionIdentityAsync(
            long divisionId,
            Optional<long> guildId = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null)
                .Where(x => x.DivisionId == divisionId);

            if (guildId.IsSpecified)
                query = query.Where(x => x.Division.GuildId == guildId.Value);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            var result = await query
                .Select(CharacterGuildDivisionIdentityViewModel.FromVersionEntityProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return (result is null)
                ? guildId.IsSpecified
                    ? new DataNotFoundError($"Guild ID {guildId.Value}, Division ID {divisionId}").ToError<CharacterGuildDivisionIdentityViewModel>()
                    : new DataNotFoundError($"Division ID {divisionId}").ToError<CharacterGuildDivisionIdentityViewModel>()
                : result.ToSuccess();
        }

        public async Task<OperationResult<CharacterGuildIdentityViewModel>> ReadIdentityAsync(
            long guildId,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CharacterGuildVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null)
                .Where(x => x.GuildId == guildId);

            if (isDeleted.IsSpecified)
                query = query.Where(x => x.IsDeleted == isDeleted.Value);

            var result = await query
                .Select(CharacterGuildIdentityViewModel.FromVersionEntityProjection)
                .FirstOrDefaultAsync(cancellationToken);

            return (result is null)
                ? new DataNotFoundError($"Guild ID {guildId}").ToError<CharacterGuildIdentityViewModel>()
                : result.ToSuccess();
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
                return new DataNotFoundError($"Character Guild ID {guildId}")
                    .ToError<long>();

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
                return new NoChangesGivenError($"Character Guild ID {guildId}")
                    .ToError<long>();
            }

            currentVersion.NextVersion = newVersion;
            await _context.AddAsync(newVersion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return newVersion.Id
                .ToSuccess();
        }

        public async Task<OperationResult<long>> UpdateDivisionAsync(
            long divisionId,
            long actionId,
            Optional<string> name = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var currentVersion = await _context.Set<CharacterGuildDivisionVersionEntity>()
                .AsQueryable()
                .Where(x => x.DivisionId == divisionId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
                return new DataNotFoundError($"Character Guild Division ID {divisionId}")
                    .ToError<long>();

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
                transactionScope.Complete();
                return new NoChangesGivenError($"Character Guild Division ID {divisionId}")
                    .ToError<long>();
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

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services.AddScoped<ICharacterGuildsRepository, CharacterGuildsRepository>();
    }
}