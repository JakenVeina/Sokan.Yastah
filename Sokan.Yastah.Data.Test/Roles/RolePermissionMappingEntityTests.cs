using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RolePermissionMappingEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             roleId,         permissionId,   creationId,     deletionId      */
                new TestCaseData(   default(long),  default(long),  default(int),   default(long),  default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  long.MinValue,  int.MinValue,   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3,              4L,             5L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             7L,             8,              9L,             10L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   11L,            12L,            13,             14L,            15L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  int.MaxValue,   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            long roleId,
            int permissionId,
            long creationId,
            long? deletionId)
        {
            var result = new RolePermissionMappingEntity(
                id,
                roleId,
                permissionId,
                creationId,
                deletionId);

            result.Id.ShouldBe(id);
            result.RoleId.ShouldBe(roleId);
            result.PermissionId.ShouldBe(permissionId);
            result.CreationId.ShouldBe(creationId);
            result.DeletionId.ShouldBe(deletionId);
        }

        #endregion Constructor() Tests
    }
}
