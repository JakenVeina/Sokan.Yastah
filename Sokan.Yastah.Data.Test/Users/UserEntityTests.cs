using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserEntityTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             username,       discriminator,      avatarHash,         firstSeen,                          lastSeen                            */
                new TestCaseData(   default(ulong), string.Empty,   string.Empty,       default(string?),   default(DateTimeOffset),            default(DateTimeOffset)             ).SetName("{m}(Default Values"),
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,       string.Empty,       DateTimeOffset.MinValue,            DateTimeOffset.MinValue             ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "username 2",   "discriminator 3",  "avatarHash 4",     DateTimeOffset.Parse("0005-06-07"), DateTimeOffset.Parse("0008-09-10")  ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   11UL,           "username 12",  "discriminator 13", "avatarHash 14",    DateTimeOffset.Parse("0015-04-17"), DateTimeOffset.Parse("0018-07-20")  ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   21UL,           "username 22",  "discriminator 23", "avatarHash 24",    DateTimeOffset.Parse("0025-02-27"), DateTimeOffset.Parse("0028-05-30")  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, string.Empty,   string.Empty,       string.Empty,       DateTimeOffset.MaxValue,            DateTimeOffset.MaxValue             ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            ulong id,
            string username,
            string discriminator,
            string? avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen)
        {
            var result = new UserEntity(
                id,
                username,
                discriminator,
                avatarHash,
                firstSeen,
                lastSeen);

            result.Id.ShouldBe(id);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.FirstSeen.ShouldBe(firstSeen);
            result.LastSeen.ShouldBe(lastSeen);
        }

        #endregion Constructor() Tests
    }
}
