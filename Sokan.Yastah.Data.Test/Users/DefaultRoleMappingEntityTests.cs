using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class DefaultRoleMappingEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             roleId,         creationId,     deletionId      */
                new TestCaseData(   default(long),  default(long),  default(long),  default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  long.MinValue,  long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6L,             7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10L,            11L,            12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            long roleId,
            long creationId,
            long? deletionId)
        {
            var result = new DefaultRoleMappingEntity(
                id,
                roleId,
                creationId,
                deletionId);

            result.Id.ShouldBe(id);
            result.RoleId.ShouldBe(roleId);
            result.CreationId.ShouldBe(creationId);
            result.DeletionId.ShouldBe(deletionId);
        }

        #endregion Constructor() Tests
    }
}
