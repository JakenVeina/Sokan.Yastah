using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data.Authentication;

namespace Sokan.Yastah.Data.Test.Authentication
{
    [TestFixture]
    public class AuthenticationTicketsRepositoryTests
    {
        #region Test Context

        internal class TestContext
            : MockYastahDbTestContext
        {
            public static TestContext CreateReadOnly()
                => new TestContext(
                    AuthenticationTicketsTestEntitySetBuilder.SharedSet);

            private TestContext(
                    YastahTestEntitySet entities)
                : base(
                    entities) { }

            public AuthenticationTicketsRepository BuildUut()
                => new AuthenticationTicketsRepository(
                    MockContext.Object,
                    LoggerFactory.CreateLogger<AuthenticationTicketsRepository>());
        }

        #endregion Test Context

        #region AsyncEnumerateIdentities() Tests

        internal static readonly IReadOnlyList<TestCaseData> AsyncEnumerateIdentities_TestCaseData
            = new[]
            {
                /*                  isDeleted                           ticketIds                       */
                new TestCaseData(   Optional<bool>.Unspecified,         new[] { 1L, 2L, 3L, 4L, 5L })   .SetName("{m}()"),
                new TestCaseData(   Optional<bool>.FromValue(true),     new[] { 1L, 2L             })   .SetName("{m}(isDeleted: true)"),
                new TestCaseData(   Optional<bool>.FromValue(false),    new[] {         3L, 4L, 5L })   .SetName("{m}(isDeleted: false)")
            };

        [TestCaseSource(nameof(AsyncEnumerateIdentities_TestCaseData))]
        public async Task AsyncEnumerateIdentities_Always_ReturnsMatches(
            Optional<bool> isDeleted,
            IReadOnlyList<long> ticketIds)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var results = await uut.AsyncEnumerateIdentities(
                    isDeleted)
                .ToArrayAsync();

            results.ShouldNotBeNull();
            results.ForEach(result => result.ShouldNotBeNull());
            results.Select(x => x.Id).ShouldBeSetEqualTo(ticketIds);
            results.ForEach(result =>
            {
                var entity = testContext.Entities.AuthenticationTickets.First(x => x.Id == result.Id);

                result.UserId.ShouldBe(entity.UserId);
            });

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion ReadIdentitiesAsync() Tests

        #region CreateAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> CreateAsync_TestCaseData
            = new[]
            {
                /*                  userId          actionId        id              */
                new TestCaseData(   ulong.MinValue, long.MinValue,  long.MinValue)  .SetName("{m}(Min Values)"),
                new TestCaseData(   1UL,            4L,             7L)             .SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   2UL,            5L,             8L)             .SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   3UL,            6L,             9L)             .SetName("{m}(Unique Value Set 3)"),
                new TestCaseData(   ulong.MaxValue, long.MaxValue,  long.MaxValue)  .SetName("{m}(Max Values)"),
            };

        [TestCaseSource(nameof(CreateAsync_TestCaseData))]
        public async Task CreateAsync_Always_AddsNewEntity(
            ulong userId,
            long actionId,
            long id)
        {
            using var testContext = TestContext.CreateReadOnly();

            testContext.MockContext
                .Setup(x => x.AddAsync(It.IsAny<AuthenticationTicketEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AuthenticationTicketEntity, CancellationToken>((x, y) => x.Id = id);

            var uut = testContext.BuildUut();

            var result = await uut.CreateAsync(
                userId,
                actionId,
                testContext.CancellationToken);

            testContext.MockContext
                .ShouldHaveReceived(x => x.AddAsync(It.IsNotNull<AuthenticationTicketEntity>(), testContext.CancellationToken));
            testContext.MockContext
                .ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));

            var entity = testContext.MockContext
                .Invocations
                .Where(x => x.Method.Name == nameof(YastahDbContext.AddAsync))
                .Select(x => (AuthenticationTicketEntity)x.Arguments[0])
                .First();

            entity.UserId.ShouldBe(userId);
            entity.CreationId.ShouldBe(actionId);

            result.ShouldBe(id);
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketDoesNotExist_TestCaseData
            = new[]
            {
                /*                  ticketId,       actionId        */
                new TestCaseData(   default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   1L,             2L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   3L,             4L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   5L,             6L              ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketDoesNotExist_TestCaseData))]
        public async Task DeleteAsync_TicketDoesNotExist_ReturnsDataNotFound(
            long ticketId,
            long actionId)
        {
            using var testContext = TestContext.CreateReadOnly();

            testContext.MockContext
                .Setup(x => x.FindAsync<AuthenticationTicketEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as AuthenticationTicketEntity);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                ticketId,
                actionId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataNotFoundError>();
            result.Error.Message.ShouldContain(ticketId.ToString());

            testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity?>(
                It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                testContext.CancellationToken));
            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketIsDeleted_TestCaseData
            = new[]
            {
                /*                  ticketId,       userId,         creationId,     deletionId,     actionId        */
                new TestCaseData(   default(long),  default(ulong), default(long),  default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, long.MinValue,  long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, long.MaxValue,  long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   1L,             2UL,            3L,             4L,             5L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             7UL,            8L,             9L,             10L             ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   11L,            12UL,           13L,            14L,            15L             ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketIsDeleted_TestCaseData))]
        public async Task DeleteAsync_TicketIsDeleted_ReturnsDataAlreadyDeleted(
            long ticketId,
            ulong userId,
            long creationId,
            long deletionId,
            long actionId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var ticket = new AuthenticationTicketEntity(
                id: ticketId,
                userId: userId,
                creationId: creationId,
                deletionId: deletionId);
            testContext.MockContext
                .Setup(x => x.FindAsync<AuthenticationTicketEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                ticketId,
                actionId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeOfType<DataAlreadyDeletedError>();
            result.Error.Message.ShouldContain(ticketId.ToString());

            ticket.DeletionId.ShouldBe(deletionId);

            testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity?>(
                It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                testContext.CancellationToken));
            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketIsNotDeleted_TestCaseData
            = new[]
            {
                /*                  ticketId,       userId,         creationId,     actionId        */
                new TestCaseData(   default(long),  default(ulong), default(long),  default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   long.MinValue,  ulong.MinValue, long.MinValue,  long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   long.MaxValue,  ulong.MaxValue, long.MaxValue,  long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   1L,             2UL,            3L,             4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   5L,             6UL,            7L,             8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   9L,             10UL,           11L,            12L             ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketIsNotDeleted_TestCaseData))]
        public async Task DeleteAsync_Otherwise_DeletesTicketAndReturnsSuccess(
            long ticketId,
            ulong userId,
            long creationId,
            long actionId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var ticket = new AuthenticationTicketEntity(
                id:         ticketId,
                userId:     userId,
                creationId: creationId,
                deletionId: null);
            testContext.MockContext
                .Setup(x => x.FindAsync<AuthenticationTicketEntity?>(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            var uut = testContext.BuildUut();

            var result = await uut.DeleteAsync(
                ticketId,
                actionId,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();

            ticket.DeletionId.ShouldBe(actionId);

            testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity?>(
                It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                testContext.CancellationToken));
            testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));
        }

        #endregion DeleteAsync() Tests

        #region ReadActiveIdAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> ReadActiveIdAsync_TicketDoesNotExist_TestCaseData
            = new[]
            {
                /*                  userId          */
                new TestCaseData(   ulong.MinValue) .SetName("{m}(Min Values)"),
                new TestCaseData(   ulong.MaxValue) .SetName("{m}(Max Values)"),
                new TestCaseData(   4UL)            .SetName("{m}(Unique Value Set)")
            };

        [TestCaseSource(nameof(ReadActiveIdAsync_TicketDoesNotExist_TestCaseData))]
        public async Task ReadActiveIdAsync_TicketDoesNotExist_ReturnsFailure(
            ulong userId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.ReadActiveIdAsync(
                userId,
                testContext.CancellationToken);

            result.IsFailure.ShouldBeTrue();
            result.Error.Message.ShouldContain(userId.ToString());

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        internal static readonly IReadOnlyList<TestCaseData> ReadActiveIdAsync_TicketExists_TestCaseData
            = new[]
            {
                /*                  userId      ticketId    */
                new TestCaseData(   1UL,        3L)         .SetName("{m}(userId: 1)"),
                new TestCaseData(   2UL,        5L)         .SetName("{m}(userId: 2)"),
                new TestCaseData(   3UL,        4L)         .SetName("{m}(userId: 3)")
            };

        [TestCaseSource(nameof(ReadActiveIdAsync_TicketExists_TestCaseData))]
        public async Task ReadActiveIdAsync_TicketExists_ReturnsSuccessWithTicketId(
            ulong userId,
            long ticketId)
        {
            using var testContext = TestContext.CreateReadOnly();

            var uut = testContext.BuildUut();

            var result = await uut.ReadActiveIdAsync(
                userId,
                testContext.CancellationToken);

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(ticketId);

            testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        #endregion ReadActiveIdAsync() Tests
    }
}
