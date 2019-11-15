using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    [TestFixture]
    public class PermissionIdentityViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             name,           */
                new TestCaseData(   default(int),   string.Empty    ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue,   string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              "name 2"        ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3,              "name 4"        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5,              "name 6"        ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int id,
            string name)
        {
            var result = new PermissionIdentityViewModel(
                id,
                name);

            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
        }

        #endregion Constructor() Tests
    }
}
