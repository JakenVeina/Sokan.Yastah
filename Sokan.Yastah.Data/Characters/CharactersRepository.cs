using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.EntityFrameworkCore;

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
            decimal sanityValue,
            long creationId,
            CancellationToken cancellationToken);

        Task<OperationResult<long>> UpdateAsync(
            long characterId,
            long actionId,
            Optional<string> name = default,
            Optional<long> divisionId = default,
            Optional<decimal> experiencePoints = default,
            Optional<decimal> goldAmount = default,
            Optional<decimal> sanityValue = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default);
    }

    public class CharactersRepository
        : ICharactersRepository
    {
        public CharactersRepository(
            YastahDbContext context,
            ITransactionScopeFactory transactionScopeFactory
            )
        {
            _context = context;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task<long> CreateAsync(
            ulong ownerId,
            string name,
            long divisionId,
            decimal experiencePoints,
            decimal goldAmount,
            decimal sanityValue,
            long creationId,
            CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var character = new CharacterEntity(
                id:         default,
                ownerId:    ownerId);

            await _context.AddAsync(character, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var version = new CharacterVersionEntity(
                id:                 default,
                characterId:        character.Id,
                divisionId:         divisionId,
                name:               name,
                experiencePoints:   experiencePoints,
                goldAmount:         goldAmount,
                sanityValue:        sanityValue,
                isDeleted:          false,
                creationId:         creationId,
                previousVersionId:  null,
                nextVersionId:      null);

            await _context.AddAsync(version, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            transactionScope.Complete();

            return character.Id;
        }

        public async Task<OperationResult<long>> UpdateAsync(
            long characterId,
            long actionId,
            Optional<string> name = default,
            Optional<long> divisionId = default,
            Optional<decimal> experiencePoints = default,
            Optional<decimal> goldAmount = default,
            Optional<decimal> sanityValue = default,
            Optional<bool> isDeleted = default,
            CancellationToken cancellationToken = default)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var currentVersion = await _context.Set<CharacterVersionEntity>()
                .AsQueryable()
                .Where(x => x.CharacterId == characterId)
                .Where(x => x.NextVersionId == null)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentVersion is null)
                return new DataNotFoundError($"Character ID {characterId}")
                    .ToError<long>();

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
                sanityValue:        sanityValue.IsSpecified
                                        ? sanityValue.Value
                                        : currentVersion.SanityValue,
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
                && (newVersion.SanityValue == currentVersion.SanityValue)
                && (newVersion.IsDeleted == currentVersion.IsDeleted))
            {
                transactionScope.Complete();
                return new NoChangesGivenError($"Character ID {characterId}")
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
