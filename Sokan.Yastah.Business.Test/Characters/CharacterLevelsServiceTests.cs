using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterLevelsServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodWithLoggerFactoryAndMemoryCacheTestContext
        {
            public TestContext()
            {
                UtcNow = default;
                NextAdministrationActionId = default;

                MockAdministrationActionsRepository = new Mock<IAdministrationActionsRepository>();
                MockAdministrationActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong?>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockCharacterLevelsRepository = new Mock<ICharacterLevelsRepository>();

                MockSystemClock = new Mock<ISystemClock>();
                MockSystemClock
                    .Setup(x => x.UtcNow)
                    .Returns(() => UtcNow);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);

                MockTransactionScope = new Mock<ITransactionScope>();
            }

            public DateTimeOffset UtcNow;
            public long NextAdministrationActionId;

            public readonly Mock<IAdministrationActionsRepository> MockAdministrationActionsRepository;
            public readonly Mock<ICharacterLevelsRepository> MockCharacterLevelsRepository;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharacterLevelsService BuildUut()
                => new CharacterLevelsService(
                    MockAdministrationActionsRepository.Object,
                    MockCharacterLevelsRepository.Object,
                    LoggerFactory.CreateLogger<CharacterLevelsService>(),
                    MemoryCache,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object);

            public void CacheExperienceThresholds(IReadOnlyList<int> experienceThresholds)
                => MemoryCache.Set(CharacterLevelsService._getCurrentDefinitionsCacheKey, experienceThresholds
                    .Select((experienceThreshold, index) => new CharacterLevelDefinitionViewModel(
                        index + 1,
                        experienceThreshold))
                    .ToArray());

            public void SetCurrentDefinitionsCache(IReadOnlyList<CharacterLevelDefinitionViewModel> definitions)
                => MemoryCache.Set(CharacterLevelsService._getCurrentDefinitionsCacheKey, definitions);
        }

        #endregion Test Context

        #region GetCurrentDefinitionsAsync() Tests

        [Test]
        public async Task GetCurrentDefinitionsAsync_DefinitionsAreNotCached_ReadsDefinitions()
        {
            using var testContext = new TestContext();

            var definitions = new CharacterLevelDefinitionViewModel[]
            {
                new CharacterLevelDefinitionViewModel(
                    level:                  default,
                    experienceThreshold:    default)
            };

            testContext.MockCharacterLevelsRepository
                .Setup(x => x.AsyncEnumerateDefinitions(
                    It.IsAny<Optional<bool>>()))
                .Returns(definitions.ToAsyncEnumerable());

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentDefinitionsAsync(
                testContext.CancellationToken);

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .AsyncEnumerateDefinitions(false));

            testContext.MemoryCache.TryGetValue(CharacterLevelsService._getCurrentDefinitionsCacheKey, out var cacheValue)
                .ShouldBeTrue();
            cacheValue.ShouldBeAssignableTo<IReadOnlyCollection<CharacterLevelDefinitionViewModel>>()
                .ShouldBeSetEqualTo(definitions);

            result.ShouldBeSameAs(cacheValue);
        }

        [Test]
        public async Task GetCurrentDefinitionsAsync_DefinitionsAreCached_DoesNotReadDefinitions()
        {
            using var testContext = new TestContext();

            var definitions = TestArray.Unique<CharacterLevelDefinitionViewModel>();

            testContext.SetCurrentDefinitionsCache(definitions);

            var uut = testContext.BuildUut();

            var result = await uut.GetCurrentDefinitionsAsync(
                testContext.CancellationToken);

            testContext.MockCharacterLevelsRepository.ShouldNotHaveReceived(x => x
                .AsyncEnumerateDefinitions(It.IsAny<Optional<bool>>()));

            result.ShouldBeSameAs(definitions);
        }

        #endregion GetCurrentDefinitionsAsync() Tests

        #region UpdateExperienceDiffsAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateExperienceDiffsAsync_ExperienceDiffsIsInvalid_TestCaseData
            = new[]
            {
                /*                  cachedExperienceThresholds, experienceDiffs,        performedById,  performed,                          invalidLevel,   invalidThreshold,   previousThreshold   */
                new TestCaseData(   Array.Empty<int>(),         new[] { int.MinValue }, ulong.MinValue, DateTimeOffset.MinValue,            2,              int.MinValue,       0                   ).SetName("{m}(Min Values)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { -1 },           2UL,            DateTimeOffset.Parse("2003-04-05"), 2,              -1,                 0                   ).SetName("{m}(Experience Threshold is negative)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { 0 },            6UL,            DateTimeOffset.Parse("2007-08-09"), 2,              0,                  0                   ).SetName("{m}(Experience Threshold is 0)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { 10, -1 },       11UL,           DateTimeOffset.Parse("2012-01-14"), 3,              9,                  10                  ).SetName("{m}(Experience Threshold is less than previous)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { 15, 0 },        16UL,           DateTimeOffset.Parse("2017-06-19"), 3,              15,                 15                  ).SetName("{m}(Experience Threshold is equal to previous)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { 20, -1, 21 },   22UL,           DateTimeOffset.Parse("2023-12-25"), 3,              19,                 20                  ).SetName("{m}(Experience Threshold is less than previous, and not last)"),
                new TestCaseData(   Array.Empty<int>(),         new[] { 26, 0, 27 },    28UL,           DateTimeOffset.Parse("2029-06-01"), 3,              26,                 26                  ).SetName("{m}(Experience Threshold is equal to previous, and not last)"),
                new TestCaseData(   new[] { 32, 33, 34, 35 },   new[] { 36, 0, 37 },    38UL,           DateTimeOffset.Parse("2039-04-11"), 3,              36,                 36                  ).SetName("{m}(Existing definitions is not empty)")
            };

        [TestCaseSource(nameof(UpdateExperienceDiffsAsync_ExperienceDiffsIsInvalid_TestCaseData))]
        public async Task UpdateExperienceDiffsAsync_ExperienceDiffsIsInvalid_ReturnsInvalidLevelDefinitionError(
            IReadOnlyList<int> cachedExperienceThresholds,
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            DateTimeOffset performed,
            int invalidLevel,
            int invalidThreshold,
            int previousThreshold)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed
            };

            testContext.CacheExperienceThresholds(cachedExperienceThresholds);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateExperienceDiffsAsync(
                experienceDiffs,
                performedById,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            var error = result.Error.ShouldBeOfType<InvalidLevelDefinitionError>();
            error.Level.ShouldBe(invalidLevel);
            error.ExperienceThreshold.ShouldBe(invalidThreshold);
            error.PreviousExperienceThreshold.ShouldBe(previousThreshold);

            testContext.MockTransactionScopeFactory.ShouldNotHaveReceived(x => x
                .CreateScope(It.IsAny<IsolationLevel?>()));

            testContext.MockAdministrationActionsRepository.Invocations.ShouldBeEmpty();

            testContext.MockCharacterLevelsRepository.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(CharacterLevelsService._getCurrentDefinitionsCacheKey, out _)
                .ShouldBeTrue();
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateExperienceDiffsAsync_NoChanges_TestCaseData
            = new[]
            {
                /*                  cachedExperienceThresholds, experienceDiffs,        performedById,  performed                           */
                new TestCaseData(   new[] { 0 },                Array.Empty<int>(),     default(ulong), default(DateTimeOffset)             ).SetName("{m}(Default Values)"),
                new TestCaseData(   new[] { 0, int.MaxValue },  new[] { int.MaxValue }, ulong.MaxValue, default(DateTimeOffset)             ).SetName("{m}(Max Values)"),
                new TestCaseData(   new[] { 0, 1 },             new[] { 1 },            2UL,            DateTimeOffset.Parse("2003-04-05")  ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   new[] { 0, 6, 13 },         new[] { 6, 7 },         8UL,            DateTimeOffset.Parse("2009-10-11")  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   new[] { 0, 12, 25, 39 },    new[] { 12, 13, 14 },   15UL,           DateTimeOffset.Parse("2016-05-18")  ).SetName("{m}(Unique Value Set 4)"),
                new TestCaseData(   new[] { 0, 1, 2, 3, 4 },    new[] { 1, 1, 1, 1 },   19UL,           DateTimeOffset.Parse("2020-09-22")  ).SetName("{m}(Single steps)")
            };

        [TestCaseSource(nameof(UpdateExperienceDiffsAsync_NoChanges_TestCaseData))]
        public async Task UpdateExperienceDiffsAsync_NoChangesGiven_ReturnsNoChangesGivenError(
            IReadOnlyList<int> cachedExperienceThresholds,
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            DateTimeOffset performed)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed
            };

            testContext.CacheExperienceThresholds(cachedExperienceThresholds);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateExperienceDiffsAsync(
                experienceDiffs,
                performedById,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsUpdated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            testContext.MockCharacterLevelsRepository.Invocations.ShouldBeEmpty();

            testContext.MemoryCache.TryGetValue(CharacterLevelsService._getCurrentDefinitionsCacheKey, out _)
                .ShouldBeTrue();

            testContext.MockTransactionScope.ShouldNotHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());
        }

        public static readonly IReadOnlyList<TestCaseData> UpdateExperienceDiffsAsync_ChangesGiven_TestCaseData
            = new[]
            {
                /*                  cachedExperienceThresholds, experienceDiffs,        performedById,  performed,                          actionId,   merges                                                      */
                new TestCaseData(   new[] { 0           },      new[] { 1           },  2UL,            DateTimeOffset.Parse("2003-04-05"), 6L,         new[] { (2, 1,   false)                                   } ).SetName("{m}(0 existing, 1 added)"),
                new TestCaseData(   new[] { 0, 7        },      new[] { 8           },  9UL,            DateTimeOffset.Parse("2010-11-12"), 13L,        new[] { (2, 8,   false)                                   } ).SetName("{m}(1 existing, 1 updated)"),
                new TestCaseData(   new[] { 0, 14       },      new[] { 15,  16     },  17UL,           DateTimeOffset.Parse("2018-07-20"), 21L,        new[] { (2, 15,  false), (3, 31,  false)                  } ).SetName("{m}(1 existing, 1 updated, 1 added)"),
                new TestCaseData(   new[] { 0, 22       },      new[] { 22,  23     },  24UL,           DateTimeOffset.Parse("2025-02-27"), 28L,        new[] { (3, 45,  false)                                   } ).SetName("{m}(1 existing, 1 added)"),
                new TestCaseData(   new[] { 0, 29       },      Array.Empty<int>(),     30UL,           DateTimeOffset.Parse("2031-08-02"), 34L,        new[] { (2, 29,  true)                                    } ).SetName("{m}(1 existing, 1 removed)"),
                new TestCaseData(   new[] { 0, 35,  37  },      new[] { 36,  1      },  38UL,           DateTimeOffset.Parse("2039-04-10"), 42L,        new[] { (2, 36,  false)                                   } ).SetName("{m}(2 existing, 1st updated)"),
                new TestCaseData(   new[] { 0, 43,  45  },      new[] { 44,  1,  46 },  47UL,           DateTimeOffset.Parse("2048-01-19"), 51L,        new[] { (2, 44,  false), (4, 91,  false)                  } ).SetName("{m}(2 existing, 1st updated, 1 added)"),
                new TestCaseData(   new[] { 0, 52,  53  },      new[] { 54          },  55UL,           DateTimeOffset.Parse("2056-09-27"), 59L,        new[] { (2, 54,  false), (3, 53,  true)                   } ).SetName("{m}(2 existing, 1st updated, 1 removed)"),
                new TestCaseData(   new[] { 0, 60,  61  },      new[] { 60,  62     },  63UL,           DateTimeOffset.Parse("2064-05-04"), 67L,        new[] { (3, 122, false)                                   } ).SetName("{m}(2 existing, 2nd updated)"),
                new TestCaseData(   new[] { 0, 68,  69  },      new[] { 68,  70, 71 },  72UL,           DateTimeOffset.Parse("2073-02-13"), 76L,        new[] { (3, 138, false), (4, 209, false)                  } ).SetName("{m}(2 existing, 2nd updated, 1 added)"),
                new TestCaseData(   new[] { 0, 77,  78  },      new[] { 79,  80     },  81UL,           DateTimeOffset.Parse("2082-11-22"), 85L,        new[] { (2, 79,  false), (3, 159, false)                  } ).SetName("{m}(2 existing, 2 updated)"),
                new TestCaseData(   new[] { 0, 86,  87  },      new[] { 88,  89, 90 },  91UL,           DateTimeOffset.Parse("2092-09-01"), 95L,        new[] { (2, 88,  false), (3, 177, false), (4, 267, false) } ).SetName("{m}(2 existing, 2 updated, 1 added)"),
                new TestCaseData(   new[] { 0, 96,  97  },      new[] { 96,  1,  98 },  99UL,           DateTimeOffset.Parse("2100-05-09"), 103L,       new[] { (4, 195, false)                                   } ).SetName("{m}(2 existing, 1 added)"),
                new TestCaseData(   new[] { 0, 104, 105 },      new[] { 104         },  106UL,          DateTimeOffset.Parse("2107-12-16"), 110L,       new[] { (3, 105, true)                                    } ).SetName("{m}(2 existing, 1 removed)"),
                new TestCaseData(   new[] { 0, 111, 112 },      Array.Empty<int>(),     113UL,          DateTimeOffset.Parse("2114-07-23"), 117L,       new[] { (2, 111, true),  (3, 112, true)                   } ).SetName("{m}(2 existing, 2 removed)"),
            };

        [TestCaseSource(nameof(UpdateExperienceDiffsAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateExperienceDiffsAsync_ChangesGiven_PerformsDefinitionChangesAndWipesCacheAndReturnsSuccess(
            IReadOnlyList<int> cachedExperienceThresholds,
            IReadOnlyList<int> experienceDiffs,
            ulong performedById,
            DateTimeOffset performed,
            long actionId,
            IReadOnlyList<(int level, int threshold, bool isDeleted)> merges)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.CacheExperienceThresholds(cachedExperienceThresholds);

            var uut = testContext.BuildUut();

            var result = await uut.UpdateExperienceDiffsAsync(
                experienceDiffs,
                performedById,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsUpdated,
                    performed,
                    performedById,
                    testContext.CancellationToken));

            foreach(var (level, threshold, isDeleted) in merges)
                testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                    .MergeDefinitionAsync(
                        level,
                        threshold,
                        isDeleted,
                        actionId,
                        testContext.CancellationToken));
            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .MergeDefinitionAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Exactly(merges.Count));

            testContext.MemoryCache.TryGetValue(CharacterLevelsService._getCurrentDefinitionsCacheKey, out _)
                .ShouldBeFalse();

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());
        }

        #endregion UpdateExperienceDiffsAsync() Tests
    }
}
