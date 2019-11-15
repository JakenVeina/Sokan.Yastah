using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Permissions;

namespace Sokan.Yastah.Data.Test.Permissions
{
    [TestFixture]
    public class PermissionDescriptionViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             name,           description,    */
                new TestCaseData(   default(int),   string.Empty,   string.Empty    ).SetName("{m}(Default Values"),
                new TestCaseData(   int.MinValue,   string.Empty,   string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              "name 2",       "description 3" ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4,              "name 5",       "description 6" ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7,              "name 8",       "description 9" ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   int.MaxValue,   "Max Value",    "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            int id,
            string name,
            string description)
        {
            var result = new PermissionDescriptionViewModel(
                id,
                name,
                description);

            result.Id.ShouldBe(id);
            result.Name.ShouldBe(name);
            result.Description.ShouldBe(description);
        }

        #endregion Constructor() Tests
    }
}
