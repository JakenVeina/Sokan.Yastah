using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    [TestFixture]
    public class PermissionEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  permissionId,   categoryId,     name,           description,        */
                new TestCaseData(   default(int),   default(int),   string.Empty,   string.Empty        ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue,   int.MinValue,   string.Empty,   string.Empty        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              2,              "name 3",       "description 4"     ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5,              6,              "name 7",       "description 8"     ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9,              10,             "name 11",      "description 12"    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   int.MaxValue,   "Max Value",    "Max Value"         ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int permissionId,
            int categoryId,
            string name,
            string description)
        {
            var result = new PermissionEntity(
                permissionId,
                categoryId,
                name,
                description);

            result.PermissionId.ShouldBe(permissionId);
            result.CategoryId.ShouldBe(categoryId);
            result.Name.ShouldBe(name);
            result.Description.ShouldBe(description);
        }

        #endregion Constructor() Tests
    }
}
