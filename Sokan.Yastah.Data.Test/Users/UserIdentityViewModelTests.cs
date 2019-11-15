using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserIdentityViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             username,       discriminator       */
                new TestCaseData(   default(ulong), string.Empty,   string.Empty        ).SetName("{m}(Default Values"),
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "username 2",   "discriminator 3"   ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4UL,            "username 5",   "discriminator 6"   ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7UL,            "username 8",   "discriminator 9"   ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, string.Empty,   string.Empty        ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            ulong id,
            string username,
            string discriminator)
        {
            var result = new UserIdentityViewModel(
                id,
                username,
                discriminator);

            result.Id.ShouldBe(id);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
        }

        #endregion Constructor() Tests
    }
}
