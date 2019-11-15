using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RoleDetailViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             name,           grantedPermissionIds    */
                new TestCaseData(   default(long),  string.Empty,   new int[0]              ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  string.Empty,   new int[0]              ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             "name 2",       new int[0]              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             "name 4",       new int[0]              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             "name 6",       new int[0]              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  "Max Value",    new int[0]              ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            string name,
            IReadOnlyList<int> grantedPermissionIds)
        {
            var result = new RoleDetailViewModel(
                id,
                name,
                grantedPermissionIds);

            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
            result.GrantedPermissionIds.ShouldBe(grantedPermissionIds);
        }

        #endregion Constructor() Tests
    }
}
