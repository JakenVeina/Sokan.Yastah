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
    public interface ICharacterLevelsRepository
    {
        Task<bool> AnyAsync(
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
            ITransactionScopeFactory transactionScopeFactory)
        {
            _context = context;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public Task<bool> AnyAsync(
            Optional<int> level = default,
            Optional<int> experienceThreshold = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);

            if (level.IsSpecified)
                query = query.Where(cld => cld.Level == level.Value);

            if (experienceThreshold.IsSpecified)
                query = query.Where(cld => cld.ExperienceThreshold == experienceThreshold.Value);

            if (isDeleted.IsSpecified)
                query = query.Where(cld => cld.IsDeleted == isDeleted.Value);

            return query.AnyAsync(cancellationToken);
        }

        public IAsyncEnumerable<CharacterLevelDefinitionViewModel> AsyncEnumerateDefinitions(
            Optional<bool> isDeleted = default)
        {
            var query = _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.NextVersionId == null);

            if (isDeleted.IsSpecified)
                query = query.Where(cld => cld.IsDeleted == isDeleted.Value);

            return query
                .OrderBy(cld => cld.Level)
                .Select(CharacterLevelDefinitionViewModel.FromVersionEntityProjection)
                .AsAsyncEnumerable();
        }

        public async Task<OperationResult> MergeDefinitionAsync(
            int level,
            int experienceThreshold,
            bool isDeleted,
            long actionId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

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
                var definition = new CharacterLevelDefinitionEntity(
                    level: level);

                await _context.AddAsync(definition, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                if ((newVersion.ExperienceThreshold == currentVersion.ExperienceThreshold)
                    && (newVersion.IsDeleted == currentVersion.IsDeleted))
                {
                    transactionScope.Complete();
                    return new NoChangesGivenError($"Character Level Definition {level}");
                }

                newVersion.PreviousVersionId = currentVersion.Id;
                currentVersion.NextVersion = newVersion;
            }

            await _context.AddAsync(newVersion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return OperationResult.Success;
        }

        private readonly YastahDbContext _context;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
    }
}
