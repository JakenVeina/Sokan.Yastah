using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Authentication;

namespace Sokan.Yastah.Data.Test.Authentication
{
    [TestFixture]
    public class AuthenticationTicketIdentityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         */
                new TestCaseData(   default(long),  default(ulong)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MinValue,  ulong.MinValue  ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2UL             ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6UL             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10UL            ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue  ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            long id,
            ulong userId)
        {
            var result = new AuthenticationTicketIdentity(
                id,
                userId);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
        }

        #endregion Constructor() Tests
    }
}
