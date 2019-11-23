using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    [TestFixture]
    public class CharacterEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             ownerId         */
                new TestCaseData(   default(long),  default(ulong)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             4UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             6UL             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            ulong ownerId)
        {
            var result = new CharacterEntity(
                id,
                ownerId);

            result.Id.ShouldBe(id);
            result.OwnerId.ShouldBe(ownerId);
        }

        #endregion Constructor() Tests
    }
}
