using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Characters;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class InvalidLevelDefinitionErrorTests
    {
        #region Constructor Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  level,          experienceThreshold,    previousExperienceThreshold */
                new TestCaseData(   default(int),   default(decimal),       default(decimal)            ).SetName("{m}(Default Values)"),
                new TestCaseData(   int.MinValue,   decimal.MinValue,       decimal.MinValue            ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              2M,                     3M                          ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,              5M,                     6M                          ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7,              8M,                     9M                          ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   decimal.MaxValue,       decimal.MaxValue            ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_SetsPropertiesAndInitializesMessage(
            int level,
            decimal experienceThreshold,
            decimal previousExperienceThreshold)
        {
            var uut = new InvalidLevelDefinitionError(
                level,
                experienceThreshold,
                previousExperienceThreshold);

            uut.Level.ShouldBe(level);
            uut.ExperienceThreshold.ShouldBe(experienceThreshold);
            uut.PreviousExperienceThreshold.ShouldBe(previousExperienceThreshold);
            uut.Message.ShouldContain(level.ToString());
            uut.Message.ShouldContain(experienceThreshold.ToString());
            uut.Message.ShouldContain(previousExperienceThreshold.ToString());
        }

        #endregion Constructor Tests
    }
}
