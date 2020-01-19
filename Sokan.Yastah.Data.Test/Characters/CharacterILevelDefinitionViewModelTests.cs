using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterLevelDefinitionViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  level,          experienceThreshold */
                new TestCaseData(   default(int),   default(int)        ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue,   int.MinValue        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              2                   ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3,              4                   ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5,              6                   ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   int.MaxValue        ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int level,
            int experienceThreshold)
        {
            var result = new CharacterLevelDefinitionViewModel(
                level,
                experienceThreshold);

            result.Level.ShouldBe(level);
            result.ExperienceThreshold.ShouldBe(experienceThreshold);
        }

        #endregion Constructor() Tests
    }
}
