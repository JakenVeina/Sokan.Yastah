using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterLevelDefinitionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  level           */
                new TestCaseData(   default(int)    ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2               ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3               ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int level)
        {
            var result = new CharacterLevelDefinitionEntity(
                level);

            result.Level.ShouldBe(level);
        }

        #endregion Constructor() Tests
    }
}
