using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Authentication;

namespace Sokan.Yastah.Business.Test.Authentication
{
    [TestFixture]
    public class AuthenticationTicketTests
    {
        #region Constructor() Tests

        internal static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         username,       discriminator,  avatarHash,     grantedPermissions                                                                  */
                new TestCaseData(   long.MinValue,  ulong.MinValue, string.Empty,   string.Empty,   string.Empty,   new Dictionary<int, string>()                                                       ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2UL,            "User 3",       "0004",         "00005",        new Dictionary<int, string>() { { 6, string.Empty } }                               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7L,             8UL,            "User 9",       "0010",         "00011",        new Dictionary<int, string>() { { 12, "Permission 13" } }                           ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   14L,            15UL,           "User 16",      "0017",         "00018",        new Dictionary<int, string>() { { 19, "Permission 20" }, { 21, "Permission 22" } }  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, "MaxValue",     "MaxValue",     "MaxValue",     new Dictionary<int, string>() { { int.MaxValue, "MaxValue" } }                      ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsTicket(
            long id,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            var result = new AuthenticationTicket(
                id,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);
        }

        #endregion Constructor() Tests
    }
}
