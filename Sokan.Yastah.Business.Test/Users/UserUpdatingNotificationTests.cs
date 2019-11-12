using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Users;

namespace Sokan.Yastah.Business.Test.Users
{
    [TestFixture]
    public class UserUpdatingNotificationTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  userId,         actionId        */
                new TestCaseData(   ulong.MinValue, long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3UL,            4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5UL,            6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsNotification(
            ulong userId,
            long actionId)
        {
            var result = new UserUpdatingNotification(userId, actionId);

            result.UserId.ShouldBe(userId);
            result.ActionId.ShouldBe(actionId);
        }

        #endregion Constructor() Tests
    }
}
