﻿using System;
using System.Collections.Generic;

using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Data.Users;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Data.Test.Users
{
    [TestFixture]
    public class UserDetailViewModelTests
    {
        #region Constructor() Tests

        public static readonly IReadOnlyList<TestCaseData> Constructor_TestCaseData
            = new[]
            {
                /*                  id,             username,       discriminator,      firstSeen,                          lastSeen,                           grantedPermissionIds,       deniedPermissionIds,        assignedRoleIds             */
                new TestCaseData(   default(ulong), string.Empty,   string.Empty,       default(DateTimeOffset),            default(DateTimeOffset),            TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Default Values"),
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,       DateTimeOffset.MinValue,            DateTimeOffset.MinValue,            TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "username 2",   "discriminator 3",  DateTimeOffset.Parse("0004-05-06"), DateTimeOffset.Parse("0007-08-09"), TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   10UL,           "username 11",  "discriminator 12", DateTimeOffset.Parse("0013-02-15"), DateTimeOffset.Parse("0016-05-18"), TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   19UL,           "username 20",  "discriminator 21", DateTimeOffset.Parse("0022-11-24"), DateTimeOffset.Parse("0025-02-27"), TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, string.Empty,   string.Empty,       DateTimeOffset.MaxValue,            DateTimeOffset.MaxValue,            TestArray.Unique<int>(),    TestArray.Unique<int>(),    TestArray.Unique<long>()    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(Constructor_TestCaseData))]
        public void Constructor_Always_ReturnsIdentity(
            ulong id,
            string username,
            string discriminator,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen,
            IReadOnlyList<int> grantedPermissionIds,
            IReadOnlyList<int> deniedPermissionIds,
            IReadOnlyList<long> assignedRoleIds)
        {
            var result = new UserDetailViewModel(
                id,
                username,
                discriminator,
                firstSeen,
                lastSeen,
                grantedPermissionIds,
                deniedPermissionIds,
                assignedRoleIds);

            result.Id.ShouldBe(id);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.FirstSeen.ShouldBe(firstSeen);
            result.LastSeen.ShouldBe(lastSeen);
            result.GrantedPermissionIds.ShouldBeSameAs(grantedPermissionIds);
            result.DeniedPermissionIds.ShouldBeSameAs(deniedPermissionIds);
            result.AssignedRoleIds.ShouldBeSameAs(assignedRoleIds);
        }

        #endregion Constructor() Tests
    }
}
