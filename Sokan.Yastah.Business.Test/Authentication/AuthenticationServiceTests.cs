using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NUnit.Framework;
using Moq;
using Shouldly;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using Sokan.Yastah.Business.Authentication;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Business.Roles;
using Sokan.Yastah.Business.Users;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Authentication;
using Sokan.Yastah.Data.Permissions;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Authentication
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                ActiveTicketId = 0;

                GrantedPermissions = Array.Empty<PermissionIdentityViewModel>();

                AuthorizationConfiguration = new AuthorizationConfiguration()
                {
                    AdminUserIds = Array.Empty<ulong>(),
                    MemberGuildIds = Array.Empty<ulong>()
                };

                MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

                MockAuthenticationTicketsRepository = new Mock<IAuthenticationTicketsRepository>();
                MockAuthenticationTicketsRepository
                    .Setup(x => x.ReadActiveIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => ActiveTicketId.ToSuccess());

                MockAuthorizationConfigurationOptions = new Mock<IOptions<AuthorizationConfiguration>>();
                MockAuthorizationConfigurationOptions
                    .Setup(x => x.Value)
                    .Returns(() => AuthorizationConfiguration);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() =>
                    {
                        var mockTransactionScope = new Mock<ITransactionScope>();
                        
                        MockTransactionScopes.Add(mockTransactionScope);

                        return mockTransactionScope.Object;
                    });

                MockTransactionScopes = new List<Mock<ITransactionScope>>();

                MockUsersService = new Mock<IUsersService>();
                MockUsersService
                    .Setup(x => x.GetGrantedPermissionsAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => GrantedPermissions.ToSuccess());
            }

            public long ActiveTicketId;

            public IReadOnlyCollection<PermissionIdentityViewModel> GrantedPermissions;

            public AuthorizationConfiguration AuthorizationConfiguration;

            public readonly MemoryCache MemoryCache;
            public readonly Mock<IAuthenticationTicketsRepository> MockAuthenticationTicketsRepository;
            public readonly Mock<IOptions<AuthorizationConfiguration>> MockAuthorizationConfigurationOptions;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly List<Mock<ITransactionScope>> MockTransactionScopes;
            public readonly Mock<IUsersService> MockUsersService;

            public AuthenticationService BuildUut()
                => new AuthenticationService(
                    MockAuthenticationTicketsRepository.Object,
                    MockAuthorizationConfigurationOptions.Object,
                    MemoryCache,
                    MockTransactionScopeFactory.Object,
                    MockUsersService.Object);

            public void SetActiveTicketId(ulong userId, long ticketId)
                => MockAuthenticationTicketsRepository
                    .Setup(x => x.ReadActiveIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ticketId.ToSuccess());

            public void SetNextTicketId(ulong userId, long ticketId)
                => MockAuthenticationTicketsRepository
                    .Setup(x => x.CreateAsync(
                        userId,
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(ticketId);

            public void SetGrantedPermissions(IReadOnlyDictionary<int, string> grantedPermissions)
                => GrantedPermissions = grantedPermissions
                    .Select(x => new PermissionIdentityViewModel(
                        id:     x.Key,
                        name:   x.Value))
                    .ToArray();

            public void SetNoActiveTicket(ulong userId)
                => MockAuthenticationTicketsRepository
                    .Setup(x => x.ReadActiveIdAsync(
                        userId,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new DataNotFoundError(string.Empty).ToError<long>());

            public void SetRoleMemberIds(IReadOnlyCollection<ulong> roleMemberIds)
                => MockUsersService
                    .Setup(x => x.GetRoleMemberIdsAsync(
                        It.IsAny<long>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(roleMemberIds);

            public override void Dispose()
            {
                base.Dispose();
                MemoryCache?.Dispose();
            }
        }

        #endregion Test Context

        #region Constructor() Tests

        [Test]
        public void Constructor_Always_CurrentTicketIsNull()
        {
            using var testContext = new TestContext();

            var result = testContext.BuildUut();

            result.CurrentTicket.ShouldBeNull();
        }

        #endregion Constructor() Tests

        #region OnAuthenticatedAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> OnAuthenticatedAsync_TestCaseData
            = new[]
            {
                /*                  ticketId,       userId,         username,       discriminator,  avatarHash,     grantedPermissions,                                                                                         */
                new TestCaseData(   long.MinValue,  ulong.MinValue, string.Empty,   string.Empty,   string.Empty,   new Dictionary<int, string>()                                                                               ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             4UL,            "Username 7",   "0010",         "00013",        new Dictionary<int, string>() { { 16, "Permission 19" }                                               }     ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2L,             5UL,            "Username 8",   "0011",         "00014",        new Dictionary<int, string>() { { 17, "Permission 20" }, { 21, "Permission 23" }                        }   ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3L,             6UL,            "Username 9",   "0012",         "00015",        new Dictionary<int, string>() { { 18, "Permission 21" }, { 22, "Permission 24" }, { 25, "Permission 26" } } ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, "MaxValue",     "MaxValue",     "MaxValue",     new Dictionary<int, string>()                                                                               ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnAuthenticatedAsync_TestCaseData))]
        public async Task OnAuthenticatedAsync_ActiveTicketIdIsNotCached_ReadsActiveId(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext()
            {
                ActiveTicketId = ticketId,
            };

            testContext.SetGrantedPermissions(grantedPermissions);

            var uut = testContext.BuildUut();

            var result = await uut.OnAuthenticatedAsync(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions,
                testContext.CancellationToken);

            result.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            uut.CurrentTicket.ShouldBeSameAs(result);

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .ReadActiveIdAsync(userId, testContext.CancellationToken));
        }

        [TestCaseSource(nameof(OnAuthenticatedAsync_TestCaseData))]
        public async Task OnAuthenticatedAsync_ActiveTicketIdIsCached_DoesNotReadActiveId(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext()
            {
                ActiveTicketId = ticketId
            };

            testContext.SetGrantedPermissions(grantedPermissions);

            testContext.MemoryCache.Set(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), ticketId);

            var uut = testContext.BuildUut();

            var result = await uut.OnAuthenticatedAsync(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions,
                testContext.CancellationToken);

            result.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            uut.CurrentTicket.ShouldBeSameAs(result);

            testContext.MockAuthenticationTicketsRepository.ShouldNotHaveReceived(x => x
                .ReadActiveIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()));
        }

        [TestCaseSource(nameof(OnAuthenticatedAsync_TestCaseData))]
        public async Task OnAuthenticatedAsync_TicketIdIsNotActive_GetsGrantedPermissions(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = await uut.OnAuthenticatedAsync(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions,
                testContext.CancellationToken);

            result.Id.ShouldBe(testContext.ActiveTicketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(testContext.GrantedPermissions
                .Select(x => new KeyValuePair<int, string>(x.Id, x.Name)));

            uut.CurrentTicket.ShouldBeSameAs(result);

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .GetGrantedPermissionsAsync(userId, testContext.CancellationToken));
        }

        [TestCaseSource(nameof(OnAuthenticatedAsync_TestCaseData))]
        public async Task OnAuthenticatedAsync_TicketIdIsActive_DoesNotGetGrantedPermissions(
            long ticketId,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext()
            {
                ActiveTicketId = ticketId
            };

            var uut = testContext.BuildUut();

            var result = await uut.OnAuthenticatedAsync(
                ticketId,
                userId,
                username,
                discriminator,
                avatarHash,
                grantedPermissions,
                testContext.CancellationToken);

            result.Id.ShouldBe(testContext.ActiveTicketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            uut.CurrentTicket.ShouldBeSameAs(result);

            testContext.MockUsersService.ShouldNotHaveReceived(x => x
                .GetGrantedPermissionsAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()));
        }

        #endregion OnAuthenticatedAsync() Tests

        #region OnSignInAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> OnSignInAsync_UserIsAdmin_TestCaseData
            = new[]
            {
                /*                  userId,         username,       discriminator,  avatarHash,     adminUserIds,               ticketId,       grantedPermissions                                                                  */
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,   string.Empty,   new[] { ulong.MinValue },   long.MinValue,  new Dictionary<int, string>()                                                       ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "User 2",       "0003",         "00004",        new[] { 1UL },              5L,             new Dictionary<int, string>() { { 6, string.Empty } }                               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   7UL,            "User 8",       "0009",         "00010",        new[] { 7UL, 11UL },        12L,            new Dictionary<int, string>() { { 13, "Permission 14" } }                           ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   15UL,           "User 16",      "0017",         "00018",        new[] { 19UL, 15UL, 20UL }, 21L,            new Dictionary<int, string>() { { 22, "Permission 23" }, { 24, "Permission 25" } }  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     "MaxValue",     "MaxValue",     new[] { ulong.MaxValue },   long.MaxValue,  new Dictionary<int, string>() { { int.MaxValue, "MaxValue" } }                      ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnSignInAsync_UserIsAdmin_TestCaseData))]
        public async Task OnSignInAsync_UserIsAdmin_DoesNotGetGuildIdsAndReturnsTicket(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyList<ulong> adminUserIds,
            long ticketId,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext()
            {
                ActiveTicketId = ticketId
            };
            
            testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();
            testContext.SetGrantedPermissions(grantedPermissions);

            var uut = testContext.BuildUut();

            var mockGetGuildIdsDelegate = new Mock<Func<CancellationToken, Task<IEnumerable<ulong>>>>();
            mockGetGuildIdsDelegate
                .Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ulong>());

            var result = await uut.OnSignInAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                mockGetGuildIdsDelegate.Object,
                testContext.CancellationToken);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .TrackUserAsync(userId, username, discriminator, avatarHash, testContext.CancellationToken));

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .GetGrantedPermissionsAsync(userId, testContext.CancellationToken));

            uut.CurrentTicket.ShouldBeNull();
        }

        internal static readonly IReadOnlyList<TestCaseData> OnSignInAsync_UserIsNotAdminAndNotGuildMember_TestCaseData
            = new[]
            {
                /*                  userId,         username,       discriminator,  avatarHash,     guildIds,                   adminUserIds,               memberGuildIds              */
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,   string.Empty,   new [] { ulong.MinValue },  Array.Empty<ulong>(),       Array.Empty<ulong>()        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "User 2",       "0003",         "00004",        new[] { 5UL },              new[] { 6UL },              new[] { 7UL }               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   8UL,            "User 9",       "0010",         "00011",        new[] { 12UL, 13UL },       new[] { 14UL, 15UL },       new[] { 16UL, 17UL }        ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   18UL,           "User 19",      "0020",         "00021",        new[] { 22UL, 23UL, 24UL }, new[] { 25UL, 26UL, 27UL }, new[] { 28UL, 29UL, 30UL }  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     "MaxValue",     "MaxValue",     Array.Empty<ulong>(),       Array.Empty<ulong>(),       new[] { ulong.MaxValue }    ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnSignInAsync_UserIsNotAdminAndNotGuildMember_TestCaseData))]
        public async Task OnSignInAsync_UserIsNotAdminAndNotGuildMember_ReturnsNull(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyList<ulong> guildIds,
            IReadOnlyList<ulong> adminUserIds,
            IReadOnlyList<ulong> memberGuildIds)
        {
            using var testContext = new TestContext();
            
            testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();
            testContext.AuthorizationConfiguration.MemberGuildIds = memberGuildIds.ToArray();

            var uut = testContext.BuildUut();

            var mockGetGuildIdsDelegate = new Mock<Func<CancellationToken, Task<IEnumerable<ulong>>>>();
            mockGetGuildIdsDelegate
                .Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(guildIds);

            var result = await uut.OnSignInAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                mockGetGuildIdsDelegate.Object,
                testContext.CancellationToken);

            result.ShouldBeNull();

            mockGetGuildIdsDelegate.ShouldHaveReceived(x => x
                .Invoke(testContext.CancellationToken));

            testContext.MockUsersService.ShouldNotHaveReceived(x => x
                .TrackUserAsync(It.IsAny<ulong>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

            testContext.MockUsersService.ShouldNotHaveReceived(x => x
                .GetGrantedPermissionsAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()));

            uut.CurrentTicket.ShouldBeNull();
        }

        internal static readonly IReadOnlyList<TestCaseData> OnSignInAsync_UserIsNotAdminAndIsGuildMember_TestCaseData
            = new[]
            {
                /*                  userId,         username,       discriminator,  avatarHash,     guildIds,                   adminUserIds,               memberGuildIds,             ticketId,       grantedPermissions                                                                  */
                new TestCaseData(   ulong.MinValue, string.Empty,   string.Empty,   string.Empty,   new [] { ulong.MinValue },  Array.Empty<ulong>(),       new[] { ulong.MinValue },   long.MinValue,  new Dictionary<int, string>()                                                       ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            "User 2",       "0003",         "00004",        new[] { 5UL },              new[] { 6UL },              new[] { 5UL },              7L,             new Dictionary<int, string>() { { 8, string.Empty } }                               ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   9UL,            "User 10",      "0011",         "00012",        new[] { 13UL, 14UL },       new[] { 15UL, 16UL },       new[] { 13UL, 17UL },       18L,            new Dictionary<int, string>() { { 19, "Permission 20" } }                           ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   21UL,           "User 22",      "0023",         "00024",        new[] { 25UL, 26UL, 27UL }, new[] { 28UL, 29UL, 30UL }, new[] { 31UL, 32UL, 26UL }, 33L,            new Dictionary<int, string>() { { 34, "Permission 35" }, { 36, "Permission 37" } }  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, "MaxValue",     "MaxValue",     "MaxValue",     Array.Empty<ulong>(),       new[] { ulong.MaxValue },   new[] { ulong.MaxValue },   long.MaxValue,  new Dictionary<int, string>() { { int.MaxValue, "MaxValue" } }                      ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnSignInAsync_UserIsNotAdminAndIsGuildMember_TestCaseData))]
        public async Task OnSignInAsync_UserIsNotAdminAndIsGuildMember_ReturnsTicket(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyList<ulong> guildIds,
            IReadOnlyList<ulong> adminUserIds,
            IReadOnlyList<ulong> memberGuildIds,
            long ticketId,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext();
            
            testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();
            testContext.AuthorizationConfiguration.MemberGuildIds = memberGuildIds.ToArray();
            testContext.ActiveTicketId = ticketId;
            testContext.SetGrantedPermissions(grantedPermissions);

            var uut = testContext.BuildUut();

            var mockGetGuildIdsDelegate = new Mock<Func<CancellationToken, Task<IEnumerable<ulong>>>>();
            mockGetGuildIdsDelegate
                .Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(guildIds);

            var result = await uut.OnSignInAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                mockGetGuildIdsDelegate.Object,
                testContext.CancellationToken);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .TrackUserAsync(userId, username, discriminator, avatarHash, testContext.CancellationToken));

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .GetGrantedPermissionsAsync(userId, testContext.CancellationToken));

            uut.CurrentTicket.ShouldBeNull();
        }

        [TestCaseSource(nameof(OnSignInAsync_UserIsAdmin_TestCaseData))]
        public async Task OnSignInAsync_ActiveTicketIdIsNotCached_ReadsActiveId(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyList<ulong> adminUserIds,
            long ticketId,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext()
            {
                ActiveTicketId = ticketId
            };
            
            testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();
            testContext.SetGrantedPermissions(grantedPermissions);

            var uut = testContext.BuildUut();

            var mockGetGuildIdsDelegate = new Mock<Func<CancellationToken, Task<IEnumerable<ulong>>>>();
            mockGetGuildIdsDelegate
                .Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ulong>());

            var result = await uut.OnSignInAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                mockGetGuildIdsDelegate.Object,
                testContext.CancellationToken);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            testContext.MemoryCache.TryGetValue(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), out var cacheValue)
                .ShouldBeTrue();
            cacheValue.ShouldBe(ticketId);

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .ReadActiveIdAsync(userId, testContext.CancellationToken));

            uut.CurrentTicket.ShouldBeNull();
        }

        [TestCaseSource(nameof(OnSignInAsync_UserIsAdmin_TestCaseData))]
        public async Task OnSignInAsync_ActiveTicketIdIsCached_DoesNotReadActiveId(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyList<ulong> adminUserIds,
            long ticketId,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            using var testContext = new TestContext();
            
            testContext.AuthorizationConfiguration.AdminUserIds = adminUserIds.ToArray();
            testContext.ActiveTicketId = ticketId;
            testContext.SetGrantedPermissions(grantedPermissions);

            testContext.MemoryCache.Set(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), ticketId);

            var uut = testContext.BuildUut();

            var mockGetGuildIdsDelegate = new Mock<Func<CancellationToken, Task<IEnumerable<ulong>>>>();
            mockGetGuildIdsDelegate
                .Setup(x => x.Invoke(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ulong>());

            var result = await uut.OnSignInAsync(
                userId,
                username,
                discriminator,
                avatarHash,
                mockGetGuildIdsDelegate.Object,
                testContext.CancellationToken);

            result.ShouldNotBeNull();
            result!.Id.ShouldBe(ticketId);
            result.UserId.ShouldBe(userId);
            result.Username.ShouldBe(username);
            result.Discriminator.ShouldBe(discriminator);
            result.AvatarHash.ShouldBe(avatarHash);
            result.GrantedPermissions.ShouldBeSetEqualTo(grantedPermissions);

            testContext.MockAuthenticationTicketsRepository.ShouldNotHaveReceived(x => x
                .ReadActiveIdAsync(It.IsAny<ulong>(), It.IsAny<CancellationToken>()));

            uut.CurrentTicket.ShouldBeNull();
        }

        #endregion OnSignInAsync() Tests

        #region OnNotificationPublishedAsync(RoleUpdatingNotification) Tests

        public static readonly IReadOnlyList<TestCaseData> OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasNoMembers_TestCaseData
            = new[]
            {
                /*                  roleId,         actionId        */
                new TestCaseData(   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             6L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };  

        [TestCaseSource(nameof(OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasNoMembers_TestCaseData))]
        public async Task OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasNoMembers_DoesNothing(
            long roleId,
            long actionId)
        {
            using var testContext = new TestContext();
            
            testContext.SetRoleMemberIds(Array.Empty<ulong>());

            var uut = testContext.BuildUut();

            var notification = new RoleUpdatingNotification(roleId, actionId);

            await uut.OnNotificationPublishedAsync(
                notification,
                testContext.CancellationToken);

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .GetRoleMemberIdsAsync(roleId, testContext.CancellationToken));

            testContext.MockTransactionScopeFactory.ShouldNotHaveReceived(x => x
                .CreateScope(It.IsAny<IsolationLevel?>()));
        }

        public static readonly IReadOnlyList<TestCaseData> OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasMembers_TestCaseData
            = new[]
            {
                /*                  roleId,         actionId        userTickets                                                     */
                new TestCaseData(   long.MinValue,  long.MinValue,  new[] { (ulong.MinValue, long.MinValue, long.MinValue) }        ).SetName("{m}(Min Values)"),
                new TestCaseData(   1L,             2L,             new[] { (3UL, 4L, 5L) }                                         ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             7L,             new[] { (8UL, 9L, 10L), (11UL, 12L, 13L) }                      ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   14L,            15L,            new[] { (16UL, 17L, 18L), (19UL, 20L, 21L), (22UL, 23L, 24L) }  ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue,  new[] { (ulong.MaxValue, long.MaxValue, long.MaxValue) }        ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasMembers_TestCaseData))]
        public async Task OnNotificationPublishedAsync_RoleUpdatingNotification_RoleHasMembers_DeletesActiveTicketsAndCreatesNewTickets(
            long roleId,
            long actionId,
            IReadOnlyList<(ulong userId, long activeTicketId, long newTicketId)> userTickets)
        {
            using var testContext = new TestContext();
            
            testContext.SetRoleMemberIds(userTickets.Select(x => x.userId).ToArray());
            
            foreach (var (userId, activeTicketId, newTicketId) in userTickets)
            {
                testContext.SetActiveTicketId(userId, activeTicketId);
                testContext.SetNextTicketId(userId, newTicketId);
            }

            var uut = testContext.BuildUut();

            var notification = new RoleUpdatingNotification(roleId, actionId);

            await uut.OnNotificationPublishedAsync(
                notification,
                testContext.CancellationToken);

            testContext.MockUsersService.ShouldHaveReceived(x => x
                .GetRoleMemberIdsAsync(roleId, testContext.CancellationToken));

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                    .CreateScope(It.IsAny<IsolationLevel?>()),
                Times.Exactly(userTickets.Count));

            foreach (var (userId, activeTicketId, newTicketId) in userTickets)
            {
                testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                    .ReadActiveIdAsync(
                        userId,
                        testContext.CancellationToken));

                testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                    .DeleteAsync(
                        activeTicketId,
                        actionId,
                        testContext.CancellationToken));

                testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                    .CreateAsync(
                        userId,
                        actionId,
                        testContext.CancellationToken));

                testContext.MemoryCache.TryGetValue(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), out var cacheValue)
                    .ShouldBeTrue();
                cacheValue.ShouldBe(newTicketId);
            }

            foreach (var mockTransactionScope in testContext.MockTransactionScopes)
            {
                mockTransactionScope.ShouldHaveReceived(x => x
                    .Complete());
                mockTransactionScope.ShouldHaveReceived(x => x
                    .Dispose());
            }
        }

        #endregion OnNotificationPublishedAsync(RoleUpdatingNotification) Tests

        #region OnNotificationPublishedAsync(UserInitializingNotification) Tests

        public static readonly IReadOnlyList<TestCaseData> OnNotificationPublishedAsync_UserInitializingNotification_TestCaseData
            = new[]
            {
                /*                  userId,         actionId,       ticketId        */
                new TestCaseData(   ulong.MinValue, long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2L,             3L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   4UL,            5L,             6L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   7UL,            8L,             9L              ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnNotificationPublishedAsync_UserInitializingNotification_TestCaseData))]
        public async Task OnNotificationPublishedAsync_UserInitializingNotification_Always_CreatesTicket(
            ulong userId,
            long actionId,
            long ticketId)
        {
            using var testContext = new TestContext();
            
            testContext.SetNoActiveTicket(userId);
            testContext.SetNextTicketId(userId, ticketId);

            var uut = testContext.BuildUut();

            var notification = new UserInitializingNotification(userId, actionId);

            await uut.OnNotificationPublishedAsync(
                notification,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(It.IsAny<IsolationLevel?>()));

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .ReadActiveIdAsync(
                    userId,
                    testContext.CancellationToken));

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    userId,
                    actionId,
                    testContext.CancellationToken));

            testContext.MemoryCache.TryGetValue(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), out var cacheValue)
                .ShouldBeTrue();
            cacheValue.ShouldBe(ticketId);

            testContext.MockTransactionScopes.First().ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScopes.First().ShouldHaveReceived(x => x
                .Dispose());
        }

        #endregion OnNotificationPublishedAsync(UserInitializingNotification) Tests

        #region OnNotificationPublishedAsync(UserUpdatingNotification) Tests

        public static readonly IReadOnlyList<TestCaseData> OnNotificationPublishedAsync_UserUpdatingNotification_TestCaseData
            = new[]
            {
                /*                  userId,         actionId,       activeTicketId, newTicketId     */
                new TestCaseData(   ulong.MinValue, long.MinValue,  long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            2L,             3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5UL,            6L,             7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9UL,            10L,            11L,            12L             ).SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, long.MaxValue,  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(OnNotificationPublishedAsync_UserUpdatingNotification_TestCaseData))]
        public async Task OnNotificationPublishedAsync_UserUpdatingNotification_Always_DeletesActiveTicketAndCreatesNewTicket(
            ulong userId,
            long actionId,
            long activeTicketId,
            long newTicketId)
        {
            using var testContext = new TestContext();
            
            testContext.SetActiveTicketId(userId, activeTicketId);
            testContext.SetNextTicketId(userId, newTicketId);

            var uut = testContext.BuildUut();

            var notification = new UserUpdatingNotification(userId, actionId);

            await uut.OnNotificationPublishedAsync(
                notification,
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(It.IsAny<IsolationLevel?>()));

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .ReadActiveIdAsync(
                    userId,
                    testContext.CancellationToken));

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .DeleteAsync(
                    activeTicketId,
                    actionId,
                    testContext.CancellationToken));

            testContext.MockAuthenticationTicketsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    userId,
                    actionId,
                    testContext.CancellationToken));

            testContext.MemoryCache.TryGetValue(AuthenticationService.MakeUserActiveTicketIdCacheKey(userId), out var cacheValue)
                .ShouldBeTrue();
            cacheValue.ShouldBe(newTicketId);

            testContext.MockTransactionScopes.First().ShouldHaveReceived(x => x
                .Complete());
            testContext.MockTransactionScopes.First().ShouldHaveReceived(x => x
                .Dispose());
        }

        #endregion OnNotificationPublishedAsync(UserUpdatingNotification) Tests
    }
}
