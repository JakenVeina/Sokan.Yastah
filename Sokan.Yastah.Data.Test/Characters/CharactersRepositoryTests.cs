using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharactersRepositoryTests
    {
        internal class TestContext
            : MockYastahDbTestContext
        {
            public static TestContext CreateReadOnly()
                => new TestContext(
                    CharactersTestEntitySetBuilder.SharedSet);

            public static TestContext CreateReadWrite()
                => new TestContext(
                    CharactersTestEntitySetBuilder.NewSet());

            private TestContext(
                    YastahTestEntitySet entities)
                : base(
                    entities)
            {
                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();

                MockTransactionScope = new Mock<ITransactionScope>();

                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);
            }

            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharactersRepository BuildUut()
                => new CharactersRepository(
                    MockContext.Object,
                    MockTransactionScopeFactory.Object);
        }

        #region CreateAsync() Tests

        public static IReadOnlyList<TestCaseData> CreateAsync_TestCaseData
            => new[]
            {
                /*                  ownerId,        name,           divisionId,     experiencePoints,   goldAmount,         insanityValue,      creationId,     characterId     */
                new TestCaseData(   default(ulong), string.Empty,   default(long),  default(decimal),   default(decimal),   default(decimal),   default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   ulong.MinValue, string.Empty,   long.MinValue,  decimal.MinValue,   decimal.MinValue,   decimal.MinValue,   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "name 2",       3L,             4M,                 5M,                 6M,                 7L,             8L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9UL,            "name 10",      11L,            12M,                13M,                14M,                15L,            16L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   17UL,           "name 18",      19L,            20M,                21M,                22M,                23L,            24L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     long.MaxValue,  decimal.MaxValue,   decimal.MaxValue,   decimal.MaxValue,   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateAsync_TestCaseData))]
        public async Task CreateAsync_Always_CreatesNewEntities(
            ulong ownerId,
            string name,
            long divisionId,
            decimal experiencePoints,
            decimal goldAmount,
            decimal insanityValue,
            long creationId,
            long characterId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var characterEntity = null as CharacterEntity;
            var characterVersionEntity = null as CharacterVersionEntity;

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterEntity, CancellationToken>((x, y) => characterEntity = x);
            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<CharacterVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterVersionEntity, CancellationToken>((x, y) => characterVersionEntity = x);

            testContext.MockContext
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    if (characterEntity is { })
                    {
                        characterEntity.Id = characterId;
                        if (characterVersionEntity is { })
                            characterVersionEntity.Character = characterEntity;
                    }
                });

            var uut = testContext.BuildUut();

            var result = await uut.CreateAsync(
                ownerId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                creationId,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<CharacterVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken), Times.Exactly(2));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());

            characterEntity!.Id.ShouldBe(characterId);

            characterVersionEntity!.Name.ShouldBe(name);
            characterVersionEntity.CreationId.ShouldBe(creationId);
            characterVersionEntity.PreviousVersionId.ShouldBeNull();
            characterVersionEntity.NextVersionId.ShouldBeNull();
            characterVersionEntity.CharacterId.ShouldBe(characterId);
            characterVersionEntity.Character.ShouldBeSameAs(characterEntity);

            result.ShouldBe(characterId);
        }

        #endregion CreateAsync() Tests

        #region UpdateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> UpdateAsync_GuildIdDoesNotExist_TestCaseData
            = new[]
            {
                /*                  characterId,    actionId,       name,                                               divisionId,                                 experiencePoints,                               goldAmount,                                     insanityValue,                                  isDeleted                       */
                new TestCaseData(   5L,             default(long),  default(Optional<string>),                          default(Optional<long>),                    default(Optional<decimal>),                     default(Optional<decimal>),                     default(Optional<decimal>),                     default(Optional<bool>)         ).SetName("{m}(Default values)"),
                new TestCaseData(   5L,             long.MinValue,  Optional<string>.FromValue(string.Empty),           Optional<long>.FromValue(long.MinValue),    Optional<decimal>.FromValue(decimal.MinValue),  Optional<decimal>.FromValue(decimal.MinValue),  Optional<decimal>.FromValue(decimal.MinValue),  Optional<bool>.FromValue(false) ).SetName("{m}(Min values)"),
                new TestCaseData(   5L,             1L,             Optional<string>.FromValue("Bogus Character 2"),    Optional<long>.FromValue(3L),               Optional<decimal>.FromValue(4L),                Optional<decimal>.FromValue(5L),                Optional<decimal>.FromValue(6L),                Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             7L,             Optional<string>.FromValue("Bogus Character 8"),    Optional<long>.FromValue(9L),               Optional<decimal>.FromValue(10L),               Optional<decimal>.FromValue(11L),               Optional<decimal>.FromValue(12L),               Optional<bool>.FromValue(true)  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             13L,            Optional<string>.FromValue("Bogus Character 14"),   Optional<long>.FromValue(15L),              Optional<decimal>.FromValue(16L),               Optional<decimal>.FromValue(17L),               Optional<decimal>.FromValue(18L),               Optional<bool>.FromValue(false) ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   5L,             long.MaxValue,  Optional<string>.FromValue("Bogus Character 4"),    Optional<long>.FromValue(long.MaxValue),    Optional<decimal>.FromValue(decimal.MaxValue),  Optional<decimal>.FromValue(decimal.MaxValue),  Optional<decimal>.FromValue(decimal.MaxValue),  Optional<bool>.FromValue(true)  ).SetName("{m}(Max values)"),
            };

        [TestCaseSource(nameof(UpdateAsync_GuildIdDoesNotExist_TestCaseData))]
        public async Task UpdateAsync_GuildIdDoesNotExist_ResultIsDataNotFound(
            long characterId,
            long actionId,
            Optional<string> name,
            Optional<long> divisionId,
            Optional<decimal> experiencePoints,
            Optional<decimal> goldAmount,
            Optional<decimal> insanityValue,
            Optional<bool> isDeleted)
        {
            using var testContext = TestContext.CreateReadWrite();

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                characterId,
                actionId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldNotHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(characterId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_NoChangesGiven_TestCaseData
            => new[]
            {
                /*                  characterId,    actionId,   name,                                       divisionId,                     experiencePoints,                   goldAmount,                         insanityValue,                      isDeleted                       */
                new TestCaseData(   1L,             5L,         Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Character 1, no changes)"),
                new TestCaseData(   1L,             6L,         Optional<string>.FromValue("Character 1"),  Optional<long>.FromValue(1),    Optional<decimal>.FromValue(300),   Optional<decimal>.FromValue(1200),  Optional<decimal>.FromValue(10),    Optional<bool>.FromValue(true)  ).SetName("{m}(Character 1, no differences)"),
                new TestCaseData(   2L,             7L,         Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Character 2, no changes)"),
                new TestCaseData(   2L,             8L,         Optional<string>.FromValue("Character 2a"), Optional<long>.FromValue(3),    Optional<decimal>.FromValue(550),   Optional<decimal>.FromValue(600),   Optional<decimal>.FromValue(10),    Optional<bool>.FromValue(false) ).SetName("{m}(Character 2, no differences)"),
                new TestCaseData(   3L,             9L,         Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Character 3, no changes)"),
                new TestCaseData(   3L,             10L,        Optional<string>.FromValue("Character 3b"), Optional<long>.FromValue(2),    Optional<decimal>.FromValue(550),   Optional<decimal>.FromValue(950),   Optional<decimal>.FromValue(10),    Optional<bool>.FromValue(false) ).SetName("{m}(Character 3, no differences)"),
                new TestCaseData(   4L,             11L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<bool>.Unspecified      ).SetName("{m}(Character 4, no changes)"),
                new TestCaseData(   4L,             12L,        Optional<string>.FromValue("Character 4"),  Optional<long>.FromValue(1),    Optional<decimal>.FromValue(0),     Optional<decimal>.FromValue(1000),  Optional<decimal>.FromValue(10),    Optional<bool>.FromValue(false) ).SetName("{m}(Character 4, no differences)"),
            };

        [TestCaseSource(nameof(UpdateAsync_NoChangesGiven_TestCaseData))]
        public async Task UpdateAsync_NoChangesAreNeeded_ResultIsNoChangesGiven(
            long characterId,
            long actionId,
            Optional<string> name,
            Optional<long> divisionId,
            Optional<decimal> experiencePoints,
            Optional<decimal> goldAmount,
            Optional<decimal> insanityValue,
            Optional<bool> isDeleted)
        {
            using var testContext = TestContext.CreateReadWrite();

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                characterId,
                actionId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldNotHaveReceived(x => x.AddAsync(It.IsAny<CharacterVersionEntity>(), It.IsAny<CancellationToken>()));
            testContext.MockContext
                .ShouldNotHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<NoChangesGivenError>();
            result.Error.Message.ShouldContain(characterId.ToString());
        }

        public static IReadOnlyList<TestCaseData> UpdateAsync_ChangesGiven_TestCaseData
            => new[]
            {
                /*                  characterId,    actionId,   name,                                       divisionId,                     experiencePoints,                   goldAmount,                         insanityValue,                  isDeleted,                           versionId  */
                new TestCaseData(   1L,             13L,        Optional<string>.FromValue("Character 1a"), Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          41L        ).SetName("{m}(Character 1, Name changed)"),
                new TestCaseData(   1L,             14L,        Optional<string>.Unspecified,               Optional<long>.FromValue(2),    Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          42L        ).SetName("{m}(Character 1, DivisionId changed)"),
                new TestCaseData(   1L,             15L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.FromValue(400),   Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          43L        ).SetName("{m}(Character 1, ExperiencePoints changed)"),
                new TestCaseData(   1L,             16L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(1100),  Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          44L        ).SetName("{m}(Character 1, GoldAmount changed)"),
                new TestCaseData(   1L,             17L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(9), Optional<bool>.Unspecified,          45L        ).SetName("{m}(Character 1, InsanityValue changed)"),
                new TestCaseData(   1L,             18L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.FromValue(false),     46L        ).SetName("{m}(Character 1, IsDeleted changed)"),
                new TestCaseData(   1L,             19L,        Optional<string>.FromValue("Character 1a"), Optional<long>.FromValue(2),    Optional<decimal>.FromValue(400),   Optional<decimal>.FromValue(1100),  Optional<decimal>.FromValue(9), Optional<bool>.FromValue(false),     47L        ).SetName("{m}(Character 1, all properties changed)"),
                new TestCaseData(   2L,             20L,        Optional<string>.FromValue("Character 2b"), Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          48L        ).SetName("{m}(Character 2, Name changed)"),
                new TestCaseData(   2L,             21L,        Optional<string>.Unspecified,               Optional<long>.FromValue(2),    Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          49L        ).SetName("{m}(Character 2, DivisionId changed)"),
                new TestCaseData(   2L,             22L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.FromValue(650),   Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          50L        ).SetName("{m}(Character 2, ExperiencePoints changed)"),
                new TestCaseData(   2L,             23L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(500),   Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          51L        ).SetName("{m}(Character 2, GoldAmount changed)"),
                new TestCaseData(   2L,             24L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(8), Optional<bool>.Unspecified,          52L        ).SetName("{m}(Character 2, InsanityValue changed)"),
                new TestCaseData(   2L,             25L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.FromValue(true),      53L        ).SetName("{m}(Character 2, IsDeleted changed)"),
                new TestCaseData(   2L,             26L,        Optional<string>.FromValue("Character 2b"), Optional<long>.FromValue(2),    Optional<decimal>.FromValue(650),   Optional<decimal>.FromValue(500),   Optional<decimal>.FromValue(8), Optional<bool>.FromValue(true),      54L        ).SetName("{m}(Character 2, all properties changed)"),
                new TestCaseData(   3L,             27L,        Optional<string>.FromValue("Character 3c"), Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          55L        ).SetName("{m}(Character 3, Name changed)"),
                new TestCaseData(   3L,             28L,        Optional<string>.Unspecified,               Optional<long>.FromValue(1),    Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          56L        ).SetName("{m}(Character 3, DivisionId changed)"),
                new TestCaseData(   3L,             29L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.FromValue(750),   Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          57L        ).SetName("{m}(Character 3, ExperiencePoints changed)"),
                new TestCaseData(   3L,             30L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(850),   Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          58L        ).SetName("{m}(Character 3, GoldAmount changed)"),
                new TestCaseData(   3L,             31L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(7), Optional<bool>.Unspecified,          59L        ).SetName("{m}(Character 3, InsanityValue changed)"),
                new TestCaseData(   3L,             32L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.FromValue(true),      60L        ).SetName("{m}(Character 3, IsDeleted changed)"),
                new TestCaseData(   3L,             33L,        Optional<string>.FromValue("Character 3c"), Optional<long>.FromValue(1),    Optional<decimal>.FromValue(750),   Optional<decimal>.FromValue(850),   Optional<decimal>.FromValue(7), Optional<bool>.FromValue(true),      61L        ).SetName("{m}(Character 3, all properties changed)"),
                new TestCaseData(   4L,             34L,        Optional<string>.FromValue("Character 4a"), Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          62L        ).SetName("{m}(Character 4, Name changed)"),
                new TestCaseData(   4L,             35L,        Optional<string>.Unspecified,               Optional<long>.FromValue(3),    Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          63L        ).SetName("{m}(Character 4, DivisionId changed)"),
                new TestCaseData(   4L,             36L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.FromValue(100),   Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          64L        ).SetName("{m}(Character 4, ExperiencePoints changed)"),
                new TestCaseData(   4L,             37L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(900),   Optional<decimal>.Unspecified,  Optional<bool>.Unspecified,          65L        ).SetName("{m}(Character 4, GoldAmount changed)"),
                new TestCaseData(   4L,             38L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.FromValue(6), Optional<bool>.Unspecified,          66L        ).SetName("{m}(Character 4, InsanityValue changed)"),
                new TestCaseData(   4L,             39L,        Optional<string>.Unspecified,               Optional<long>.Unspecified,     Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,      Optional<decimal>.Unspecified,  Optional<bool>.FromValue(true),      67L        ).SetName("{m}(Character 4, IsDeleted changed)"),
                new TestCaseData(   4L,             40L,        Optional<string>.FromValue("Character 4a"), Optional<long>.FromValue(3),    Optional<decimal>.FromValue(100),   Optional<decimal>.FromValue(900),   Optional<decimal>.FromValue(6), Optional<bool>.FromValue(true),      68L        ).SetName("{m}(Character 4, all properties changed)"),
            };

        [TestCaseSource(nameof(UpdateAsync_ChangesGiven_TestCaseData))]
        public async Task UpdateAsync_Otherwise_ResultIsSuccess(
            long characterId,
            long actionId,
            Optional<string> name,
            Optional<long> divisionId,
            Optional<decimal> experiencePoints,
            Optional<decimal> goldAmount,
            Optional<decimal> insanityValue,
            Optional<bool> isDeleted,
            long versionId)
        {
            using var testContext = TestContext.CreateReadWrite();

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsNotNull<CharacterVersionEntity>(), It.IsAny<CancellationToken>()))
                .Callback<CharacterVersionEntity, CancellationToken>((x, y) => x.Id = versionId);

            var previousVersionEntity = testContext.Entities.CharacterVersions.First(x => (x.CharacterId == characterId) && (x.NextVersionId is null));

            var uut = testContext.BuildUut();

            var result = await uut.UpdateAsync(
                characterId,
                actionId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                isDeleted,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory
                .ShouldHaveReceived(x => x.CreateScope(default));

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsAny<CharacterVersionEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Complete());
            testContext.MockTransactionScope
                .ShouldHaveReceived(x => x.Dispose());

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (CharacterVersionEntity)x.Arguments[0])
                .First();

            entity.ShouldNotBeNull();
            entity.CharacterId.ShouldBe(characterId);
            entity.CreationId.ShouldBe(actionId);
            entity.NextVersionId.ShouldBeNull();
            entity.PreviousVersionId.ShouldBe(previousVersionEntity.Id);
            entity.Name.ShouldBe(name.IsSpecified ? name.Value : previousVersionEntity.Name);
            entity.DivisionId.ShouldBe(divisionId.IsSpecified ? divisionId.Value : previousVersionEntity.DivisionId);
            entity.ExperiencePoints.ShouldBe(experiencePoints.IsSpecified ? experiencePoints.Value : previousVersionEntity.ExperiencePoints);
            entity.GoldAmount.ShouldBe(goldAmount.IsSpecified ? goldAmount.Value : previousVersionEntity.GoldAmount);
            entity.IsDeleted.ShouldBe(isDeleted.IsSpecified ? isDeleted.Value : previousVersionEntity.IsDeleted);

            previousVersionEntity.NextVersion.ShouldBeSameAs(entity);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(versionId);
        }

        #endregion UpdateAsync() Tests
    }
}
