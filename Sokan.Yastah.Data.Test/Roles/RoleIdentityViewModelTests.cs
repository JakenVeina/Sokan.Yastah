using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RoleIdentityViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             name            */
                new TestCaseData(   default(long),  string.Empty    ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "name 2"        ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             "name 4"        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             "name 6"        ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            string name)
        {
            var result = new RoleIdentityViewModel(
                id,
                name);

            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
        }

        #endregion Constructor() Tests
    }
}
