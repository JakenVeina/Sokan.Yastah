using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Authentication;

namespace Sokan.Yastah.Data.Test.Authentication
{
    [TestFixture]
    public class AuthenticationTicketEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             userId,         creationId,     deletionId      */
                new TestCaseData(   default(long),  default(ulong), default(long),  default(long?)  ).SetName("{m}(Default Values"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   1L,             2UL,            3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6UL,            7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10UL,           11L,            12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsEntity(
            long id,
            ulong userId,
            long creationId,
            long? deletionId)
        {
            var result = new AuthenticationTicketEntity(
                id,
                userId,
                creationId,
                deletionId);

            result.Id.ShouldBe(id);
            result.UserId.ShouldBe(userId);
            result.CreationId.ShouldBe(creationId);
            result.DeletionId.ShouldBe(deletionId);
        }

        #endregion Constructor() Tests
    }
}
