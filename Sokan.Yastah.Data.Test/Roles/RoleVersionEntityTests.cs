using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Test.Roles
{
    [TestFixture]
    public class RoleVersionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             roleId,         name,           isDeleted,      creationId,     previousVersionId,  nextVersionId   */
                new TestCaseData(   default(long),  default(long),  string.Empty,   default(bool),  default(long),  default(long?),     default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  long.MinValue,  string.Empty,   false,          long.MinValue,  long.MinValue,      long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             "name 3",       true,           4L,             5L,                 6L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7L,             8L,             "name 9",       false,          10L,            11L,                12L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   13L,            14L,            "name 15",      true,           16L,            17L,                18L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  "Max Value",    true,           long.MaxValue,  long.MaxValue,      long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            long roleId,
            string name,
            bool isDeleted,
            long creationId,
            long? previousVersionId,
            long? nextVersionId)
        {
            var result = new RoleVersionEntity(
                id,
                roleId,
                name,
                isDeleted,
                creationId,
                previousVersionId,
                nextVersionId);

            result.Id.ShouldBe(id);
            result.RoleId.ShouldBe(roleId);
            result.Name.ShouldBe(name);
            result.IsDeleted.ShouldBe(isDeleted);
            result.CreationId.ShouldBe(creationId);
            result.PreviousVersionId.ShouldBe(previousVersionId);
            result.NextVersionId.ShouldBe(nextVersionId);
        }

        #endregion Constructor() Tests
    }
}
