using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RoleEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id              */
                new TestCaseData(   default(long)   ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id)
        {
            var result = new RoleEntity(
                id);

            result.Id.ShouldBe(id);
        }

        #endregion Constructor() Tests
    }
}
