using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    [TestFixture]
    public class PermissionCategoryDescriptionViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             name,           description,        permissions                             */
                new TestCaseData(   default(int),   string.Empty,   string.Empty,       new PermissionDescriptionViewModel[0]   ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue,   string.Empty,   string.Empty,       new PermissionDescriptionViewModel[0]   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              "name 2",       "description 3",    new PermissionDescriptionViewModel[0]   ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,              "name 5",       "description 6",    new PermissionDescriptionViewModel[0]   ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7,              "name 8",       "description 9",    new PermissionDescriptionViewModel[0]   ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   "Max Value",    "Max Value",        new PermissionDescriptionViewModel[0]   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int id,
            string name,
            string description,
            IReadOnlyCollection<PermissionDescriptionViewModel> permissions)
        {
            var result = new PermissionCategoryDescriptionViewModel(
                id,
                name,
                description,
                permissions);

            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
            result.Description.ShouldBe(description);
            result.Permissions.ShouldBeSameAs(permissions);
        }

        #endregion Constructor() Tests
    }
}
