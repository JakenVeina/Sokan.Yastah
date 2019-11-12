using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Roles;

namespace Sokan.Yastah.Business.Test.Roles
{
    [TestFixture]
    public class RoleUpdatingNotificationTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  roleId,         actionId        */
                new TestCaseData(   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsNotification(
            long roleId,
            long actionId)
        {
            var result = new RoleUpdatingNotification(roleId, actionId);

            result.RoleId.ShouldBe(roleId);
            result.ActionId.ShouldBe(actionId);
        }

        #endregion Constructor() Tests
    }
}
