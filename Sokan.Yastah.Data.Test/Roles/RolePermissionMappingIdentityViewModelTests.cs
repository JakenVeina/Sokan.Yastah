using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RolePermissionMappingIdentityViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             roleId,         permissionId    */
                new TestCaseData(   default(long),  default(long),  default(int)    ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  long.MinValue,  int.MinValue    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4L,             5L,             6               ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7L,             8L,             9               ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  int.MaxValue    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            long roleId,
            int permissionId)
        {
            var result = new RolePermissionMappingIdentityViewModel(
                id,
                roleId,
                permissionId);

            result.Id.ShouldBe(id);
            result.RoleId.ShouldBe(roleId);
            result.PermissionId.ShouldBe(permissionId);
        }

        #endregion Constructor() Tests
    }
}
