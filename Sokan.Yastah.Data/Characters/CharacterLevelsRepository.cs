using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Characters
{
    public interface ICharacterLevelsRepository
    {
        Task CreateDefinitionAsnyc(
            int level,
            decimal experienceThreshold,
            long creationId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateDefinitionAsync(
            int level,
            long actionId,
            Optional<decimal> experienceThreshold = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

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

        public async Task CreateDefinitionAsnyc(
            int level,
            decimal experienceThreshold,
            long creationId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var definition = new CharacterLevelDefinitionEntity(
                level:  level);

            await _context.AddAsync(definition, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterLevelDefinitionVersionEntity(
                id:                     default,
                level:                  level,
                experienceThreshold:    experienceThreshold,
                isDeleted:              false,
                creationId:             creationId,
                previousVersionId:      null,
                nextVersionId:          null);

            await _context.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();
        }

        public async Task<OperationResult<long>> UpdateDefinitionAsync(
            int level,
            long actionId,
            Optional<decimal> experienceThreshold = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var currentVersion = await _context.Set<CharacterLevelDefinitionVersionEntity>()
                .AsQueryable()
                .Where(x => x.Level == level)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
                return new DataNotFoundError($"Character Level Definition {level}")
                    .ToError<long>();

            var newVersion = new CharacterLevelDefinitionVersionEntity(
                id:                     default,
                level:                  level,
                experienceThreshold:    experienceThreshold.IsSpecified
                                            ? experienceThreshold.Value
                                            : currentVersion.ExperienceThreshold,
                isDeleted:              isDeleted.IsSpecified
                                            ? isDeleted.Value
                                            : currentVersion.IsDeleted,
                creationId:             actionId,
                previousVersionId:      currentVersion.Id,
                nextVersionId:          null);

            if ((newVersion.ExperienceThreshold == currentVersion.ExperienceThreshold)
                && (newVersion.IsDeleted == currentVersion.IsDeleted))
            {
                transactionScope.Complete();
                return new NoChangesGivenError($"Character Level Definition {level}")
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
    }
}
