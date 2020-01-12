using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterVersionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             characterId,    name,           divisionId,     experiencePoints,   goldAmount,         insanityValue,      isDeleted,      creationId,     previousVersionId,  nextVersionId   */
                new TestCaseData(   default(long),  default(long),  string.Empty,   default(long),  default(decimal),   default(decimal),   default(decimal),   default(bool),  default(long),  default(long?),     default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  long.MinValue,  string.Empty,   long.MinValue,  decimal.MinValue,   decimal.MinValue,   decimal.MinValue,   false,          long.MinValue,  long.MinValue,      long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             "name 3",       4L,             5M,                 6M,                 7M,                 true,           8L,             9L,                 10L             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   11L,            12L,            "name 13",      14L,            15M,                16M,                17M,                false,          18L,            19L,                20L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   21L,            22L,            "name 23",      24L,            25M,                26M,                27M,                true,           28L,            29L,                30L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  "MaxValue",     long.MaxValue,  decimal.MaxValue,   decimal.MaxValue,   decimal.MaxValue,   true,           long.MaxValue,  long.MaxValue,      long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            long characterId,
            string name,
            long divisionId,
            decimal experiencePoints,
            decimal goldAmount,
            decimal insanityValue,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            var result = new CharacterVersionEntity(
                id,
                characterId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                isDeleted,
                creationId,
                previousVersionId,
                nextVersionId);

            result.Id.ShouldBe(id);
            result.CharacterId.ShouldBe(characterId);
            result.Name.ShouldBe(name);
            result.DivisionId.ShouldBe(divisionId);
            result.ExperiencePoints.ShouldBe(experiencePoints);
            result.GoldAmount.ShouldBe(goldAmount);
            result.InsanityValue.ShouldBe(insanityValue);
            result.IsDeleted.ShouldBe(isDeleted);
            result.CreationId.ShouldBe(creationId);
            result.PreviousVersionId.ShouldBe(previousVersionId);
            result.NextVersionId.ShouldBe(nextVersionId);
        }

        #endregion Constructor() Tests
    }
}
