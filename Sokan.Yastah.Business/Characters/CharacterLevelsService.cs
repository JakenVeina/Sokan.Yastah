using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Auditing;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Auditing;
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

    [ServiceBinding(ServiceLifetime.Scoped)]
    public class CharacterLevelsService
        : ICharacterLevelsService
    {
        public CharacterLevelsService(
            IAuditableActionsRepository auditableActionsRepository,
            ICharacterLevelsRepository characterLevelsRepository,
            ILogger<CharacterLevelsService> logger,
            IMemoryCache memoryCache,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _auditableActionsRepository = auditableActionsRepository;
            _characterLevelsRepository = characterLevelsRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public ValueTask<IReadOnlyList<CharacterLevelDefinitionViewModel>> GetCurrentDefinitionsAsync(
                CancellationToken cancellationToken)
            => _memoryCache.OptimisticGetOrCreateAsync(_getCurrentDefinitionsCacheKey, async entry =>
            {
                CharactersLogMessages.CharacterLevelDefinitionsFetchingCurrent(_logger);

                entry.Priority = CacheItemPriority.High;

                var definitions = await _characterLevelsRepository.AsyncEnumerateDefinitions(
                            isDeleted: false)
                        .ToArrayAsync(cancellationToken)
                    as IReadOnlyList<CharacterLevelDefinitionViewModel>;
                CharactersLogMessages.CharacterLevelDefinitionsFetchedCurrent(_logger);

                return definitions;
            });

        public async Task<OperationResult> UpdateExperienceDiffsAsync(
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            CancellationToken cancellationToken)
        {
            CharactersLogMessages.CharacterLevelDefinitionsUpdating(_logger);

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
            {
                CharactersLogMessages.CharacterLevelDefinitionProposed(_logger, level, experienceThreshold, previousExperienceThreshold);
                if (experienceThreshold <= previousExperienceThreshold)
                {
                    CharactersLogMessages.CharacterLevelDefinitionValidationFailed(_logger, level, experienceThreshold, previousExperienceThreshold);
                    return new InvalidLevelDefinitionError(
                        level,
                        experienceThreshold,
                        previousExperienceThreshold);
                }
            }

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var currentDefinitions = await GetCurrentDefinitionsAsync(cancellationToken);
            CharactersLogMessages.CharacterLevelDefinitionsFetchedCurrent(_logger);

            var sequenceLength = Math.Max(experienceDiffs.Count + 1, currentDefinitions.Count);
            var pairwiseSequence = Enumerable.Zip(
                currentDefinitions
                    .PadEnd<CharacterLevelDefinitionViewModel?>(sequenceLength, null),
                proposedDefinitions
                    .Select(x => x.ToNullable())
                    .PadEnd(sequenceLength, null),
                (current, proposed) => (current, proposed));

            var actionId = await _auditableActionsRepository.CreateAsync(
                (int)CharacterManagementAdministrationActionType.LevelDefinitionsUpdated,
                _systemClock.UtcNow,
                performedById,
                cancellationToken);
            AuditingLogMessages.AuditingActionCreated(_logger, actionId);

            var anyChangesMade = false;

            foreach(var (current, proposed) in pairwiseSequence)
            {
                if (proposed is null)
                {
                    CharactersLogMessages.CharacterLevelDefinitionDeleting(_logger, current!.Level);
                    await _characterLevelsRepository.MergeDefinitionAsync(
                        current!.Level,
                        current!.ExperienceThreshold,
                        true,
                        actionId,
                        cancellationToken);
                    CharactersLogMessages.CharacterLevelDefinitionDeleted(_logger, current!.Level);

                    anyChangesMade = true;
                }
                else if ((current is null) || (current.ExperienceThreshold != proposed.Value.experienceThreshold))
                {
                    CharactersLogMessages.CharacterLevelDefinitionUpdating(_logger, proposed!.Value.level, proposed!.Value.experienceThreshold);
                    await _characterLevelsRepository.MergeDefinitionAsync(
                        proposed!.Value.level,
                        proposed!.Value.experienceThreshold,
                        false,
                        actionId,
                        cancellationToken);
                    CharactersLogMessages.CharacterLevelDefinitionUpdated(_logger, proposed!.Value.level, proposed!.Value.experienceThreshold);

                    anyChangesMade = true;
                }
            }

            if (!anyChangesMade)
            {
                CharactersLogMessages.CharacterLevelDefinitionsNoChangesGiven(_logger);
                return new NoChangesGivenError("Character Level Definitions");
            }

            transactionScope.Complete();
            TransactionsLogMessages.TransactionScopeCommitted(_logger);

            _memoryCache.Remove(_getCurrentDefinitionsCacheKey);
            CharactersLogMessages.CharacterLevelDefinitionsCacheCleared(_logger);

            return OperationResult.Success;
        }

        private readonly IAuditableActionsRepository _auditableActionsRepository;
        private readonly ICharacterLevelsRepository _characterLevelsRepository;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        internal const string _getCurrentDefinitionsCacheKey
            = nameof(CharacterLevelsService) + "." + nameof(GetCurrentDefinitionsAsync);
    }
}
