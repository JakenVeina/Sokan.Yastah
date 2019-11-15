using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Test.Administration
{
    [TestFixture]
    public class AdministrationActionTypeEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             categoryId,     name            */
                new TestCaseData(   default(int),   default(int),   string.Empty    ).SetName("{m}(Default Values)"),
                new TestCaseData(   int.MinValue,   int.MinValue,   string.Empty    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1,              2,              "name 3"        ).SetName("{m}(Unique Value Set 1"),
                new TestCaseData(   4,              5,              "name 6"        ).SetName("{m}(Unique Value Set 2"),
                new TestCaseData(   7,              8,              "name 9"        ).SetName("{m}(Unique Value Set 3"),
                new TestCaseData(   int.MaxValue,   int.MaxValue,   "Max Value"     ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsEntity(
            int id,
            int categoryId,
            string name)
        {
            var result = new AdministrationActionTypeEntity(
                id,
                categoryId,
                name);

            result.Id.ShouldBe(id);
            result.CategoryId.ShouldBe(categoryId);
            result.Name.ShouldBe(name);
        }

        #endregion Constructor() Tests
    }
}
