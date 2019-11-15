using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserPermissionMappingEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         permissionId,   isDenied,       creationId,     deletionId      */
                new TestCaseData(   default(long),  default(ulong), default(int),   default(bool),  default(long),  default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, int.MinValue,   false,          long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             1UL,            2,              true,           3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             5UL,            6,              false,          7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             9UL,            10,             true,           11L,            12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, int.MaxValue,   true,           long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            ulong userId,
            int permissionId,
            bool isDenied,
            long creationId,
            long? deletionId)
        {
            var result = new UserPermissionMappingEntity(
                id,
                userId,
                permissionId,
                isDenied,
                creationId,
                deletionId);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
            result.PermissionId.ShouldBe(permissionId);
            result.IsDenied.ShouldBe(isDenied);
            result.CreationId.ShouldBe(creationId);
            result.DeletionId.ShouldBe(deletionId);
        }

        #endregion Constructor() Tests
    }
}
