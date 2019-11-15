using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserPermissionMappingIdentityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         permissionId,   isDenied        */
                new TestCaseData(   default(long),  default(ulong), default(int),   default(bool)   ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, int.MinValue,   false           ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             1UL,            2,              true            ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             5UL,            6,              false           ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             9UL,            10,             true            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, int.MaxValue,   true            ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            ulong userId,
            int permissionId,
            bool isDenied)
        {
            var result = new UserPermissionMappingIdentity(
                id,
                userId,
                permissionId,
                isDenied);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
            result.PermissionId.ShouldBe(permissionId);
            result.IsDenied.ShouldBe(isDenied);
        }

        #endregion Constructor() Tests
    }
}
