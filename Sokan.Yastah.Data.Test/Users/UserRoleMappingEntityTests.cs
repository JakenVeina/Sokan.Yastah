using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserRoleMappingEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         roleId,         creationId,     deletionId      */
                new TestCaseData(   default(long),  default(ulong), default(long),  default(long),  default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, long.MinValue,  long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2UL,            3L,             4L,             5L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             7UL,            8L,             9L,             10L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   11L,            12UL,           13L,            14L,            15L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, long.MaxValue,  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            ulong userId,
            long roleId,
            long creationId,
            long? deletionId)
        {
            var result = new UserRoleMappingEntity(
                id,
                userId,
                roleId,
                creationId,
                deletionId);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
            result.RoleId.ShouldBe(roleId);
            result.CreationId.ShouldBe(creationId);
            result.DeletionId.ShouldBe(deletionId);
        }

        #endregion Constructor() Tests
    }
}
