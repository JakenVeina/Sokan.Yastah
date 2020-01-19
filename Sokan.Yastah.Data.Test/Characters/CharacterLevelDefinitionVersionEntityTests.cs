using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterLevelDefinitionVersionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             level,          experienceThreshold,    isDeleted,      creationId,     previousVersionId,  nextVersionId   */
                new TestCaseData(   default(long),  default(int),   default(int),           default(bool),  default(long),  default(long?),     default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  int.MinValue,   int.MinValue,           false,          long.MinValue,  long.MinValue,      long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2,              3,                      true,           4L,             5L,                 6L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7L,             8,              9,                      false,          10L,            11L,                12L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   13L,            14,             15,                     true,           16L,            17L,                18L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  int.MaxValue,   int.MaxValue,           true,           long.MaxValue,  long.MaxValue,      long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            int level,
            int experienceThreshold,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            var result = new CharacterLevelDefinitionVersionEntity(
                id,
                level,
                experienceThreshold,
                isDeleted,
                creationId,
                previousVersionId,
                nextVersionId);

            result.Id.ShouldBe(id);
            result.Level.ShouldBe(level);
            result.ExperienceThreshold.ShouldBe(experienceThreshold);
            result.IsDeleted.ShouldBe(isDeleted);
            result.CreationId.ShouldBe(creationId);
            result.PreviousVersionId.ShouldBe(previousVersionId);
            result.NextVersionId.ShouldBe(nextVersionId);
        }

        #endregion Constructor() Tests
    }
}
