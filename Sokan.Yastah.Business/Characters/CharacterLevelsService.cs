using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    public interface ICharacterLevelsService
    {
        ValueTask<IReadOnlyList<CharacterLevelDefinitionViewModel>> GetCurrentDefinitionsAsync(
            CancellationToken cancellationToken);

        Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            CancellationToken cancellationToken);
    }

    public class CharacterLevelsService
        : ICharacterLevelsService
    {
        public CharacterLevelsService(
            IAdministrationActionsRepository administrationActionsRepository,
            ICharacterLevelsRepository characterLevelsRepository,
            IMemoryCache memoryCache,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _characterLevelsRepository = characterLevelsRepository;
            _memoryCache = memoryCache;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public ValueTask<IReadOnlyList<CharacterLevelDefinitionViewModel>> GetCurrentDefinitionsAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(_getCurrentDefinitionsCacheKey, async entry =>
            {
                entry.Priority = CacheItemPriority.High;

                return await _characterLevelsRepository.AsyncEnumerateDefinitions(
                            isDeleted: false)
                        .ToArrayAsync(cancellationToken)
                    as IReadOnlyList<CharacterLevelDefinitionViewModel>;
            });

        public async Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            var totalExperience = 0;
            var proposedDefinitions = experienceDiffs
                .Select((experienceDiff, index) => (
                    level: index + 2,
                    previousExperienceThreshold: totalExperience,
                    experienceThreshold: totalExperience += experienceDiff))
                .Prepend((
                    level: 0,
                    previousExperienceThreshold: -1,
                    experienceThreshold: 0))
                .ToArray();

            foreach(var (level, previousExperienceThreshold, experienceThreshold) in proposedDefinitions)
                if (experienceThreshold <= previousExperienceThreshold)
                    return new InvalidLevelDefinitionError(
                        level,
                        experienceThreshold,
                        previousExperienceThreshold);

            using var transactionScope = _transactionScopeFactory.CreateScope();

            var currentDefinitions = await GetCurrentDefinitionsAsync(cancellationToken);

            var sequenceLength = Math.Max(experienceDiffs.Count + 1, currentDefinitions.Count);
            var pairwiseSequence = Enumerable.Zip(
                currentDefinitions
                    .PadEnd<CharacterLevelDefinitionViewModel?>(sequenceLength, null),
                proposedDefinitions
                    .Select(x => x.ToNullable())
                    .PadEnd(sequenceLength, null),
                (current, proposed) => (current, proposed));

            var actionId = await _administrationActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.LevelDefinitionsUpdated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);

            var anyChangesMade = false;

            foreach(var (current, proposed) in pairwiseSequence)
            {
                if (proposed is null)
                {
                    await _characterLevelsRepository.MergeDefinitionAsync(
                        current!.Level,
                        current!.ExperienceThreshold,
                        true,
                        actionId,
                        cancellationToken);

                    anyChangesMade = true;
                }
                else if ((current is null) || (current.ExperienceThreshold != proposed.Value.experienceThreshold))
                {
                    if (proposed.Value.experienceThreshold <= proposed.Value.previousExperienceThreshold)
                        return new InvalidLevelDefinitionError(
                            proposed.Value.level,
                            proposed.Value.experienceThreshold,
                            proposed.Value.previousExperienceThreshold);

                    await _characterLevelsRepository.MergeDefinitionAsync(
                        proposed!.Value.level,
                        proposed!.Value.experienceThreshold,
                        false,
                        actionId,
                        cancellationToken);

                    anyChangesMade = true;
                }
            }

            if (!anyChangesMade)
                return new NoChangesGivenError("Character Level Definitions");

            transactionScope.Complete();

            _memoryCache.Remove(_getCurrentDefinitionsCacheKey);

            return OperationResult.Success;
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly ICharacterLevelsRepository _characterLevelsRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        internal const string _getCurrentDefinitionsCacheKey
            = nameof(CharacterLevelsService) + "." + nameof(GetCurrentDefinitionsAsync);

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddScoped<ICharacterLevelsService, CharacterLevelsService>();
    }
}
