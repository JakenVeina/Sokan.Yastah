using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            public TestContext(bool isReadOnly = true)
                : base(isReadOnly) { }

            public AuthenticationTicketsRepository BuildUut()
                => new AuthenticationTicketsRepository(
                    MockContext.Object);
        }

        #endregion Test Context

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
            using (var testContext = new TestContext(isReadOnly: false))
            {
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
        }

        #endregion CreateAsync() Tests

        #region DeleteAsync() Tests

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketDoesNotExist_TestCaseData
            = new[]
            {
                /*                  ticketId        actionId        */
                new TestCaseData(   long.MinValue,  long.MinValue)  .SetName("{m}(Min Values)"),
                new TestCaseData(   0L,             7L)             .SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   6L,             13L)            .SetName("{m}(Unique Value Set 7)"),
                new TestCaseData(   long.MaxValue,  long.MaxValue)  .SetName("{m}(Max Values)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketDoesNotExist_TestCaseData))]
        public async Task DeleteAsync_TicketDoesNotExist_ReturnsDataNotFound(
            long ticketId,
            long actionId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var uut = testContext.BuildUut();

                var result = await uut.DeleteAsync(
                    ticketId,
                    actionId,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataNotFoundError>();
                result.Error.Message.ShouldContain(ticketId.ToString());

                testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity>(
                    It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                    testContext.CancellationToken));
                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketIsDeleted_TestCaseData
            = new[]
            {
                /*                  ticketId        actionId        */
                new TestCaseData(   1L,             8L)             .SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   2L,             9L)             .SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketIsDeleted_TestCaseData))]
        public async Task DeleteAsync_TicketIsDeleted_ReturnsDataAlreadyDeleted(
            long ticketId,
            long actionId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var ticket = testContext.Entities.AuthenticationTickets.First(x => x.Id == ticketId);
                var deletionId = ticket.DeletionId;

                var uut = testContext.BuildUut();

                var result = await uut.DeleteAsync(
                    ticketId,
                    actionId,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBeOfType<DataAlreadyDeletedError>();
                result.Error.Message.ShouldContain(ticketId.ToString());

                ticket.DeletionId.ShouldBe(deletionId);

                testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity>(
                    It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                    testContext.CancellationToken));
                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        public static readonly IReadOnlyList<TestCaseData> DeleteAsync_TicketIsNotDeleted_TestCaseData
            = new[]
            {
                /*                  ticketId        actionId        */
                new TestCaseData(   3L,             10L)            .SetName("{m}(Unique Value Set 4)"),
                new TestCaseData(   4L,             11L)            .SetName("{m}(Unique Value Set 5)"),
                new TestCaseData(   5L,             12L)            .SetName("{m}(Unique Value Set 6)")
            };

        [TestCaseSource(nameof(DeleteAsync_TicketIsNotDeleted_TestCaseData))]
        public async Task DeleteAsync_Otherwise_DeletesTicketAndReturnsSuccess(
            long ticketId,
            long actionId)
        {
            using (var testContext = new TestContext(isReadOnly: false))
            {
                var ticket = testContext.Entities.AuthenticationTickets.First(x => x.Id == ticketId);

                var uut = testContext.BuildUut();

                var result = await uut.DeleteAsync(
                    ticketId,
                    actionId,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();

                ticket.DeletionId.ShouldBe(actionId);

                testContext.MockContext.ShouldHaveReceived(x => x.FindAsync<AuthenticationTicketEntity>(
                    It.Is<object[]>(y => y.SequenceEqual(ticketId.ToEnumerable().Cast<object>())),
                    testContext.CancellationToken));
                testContext.MockContext.ShouldHaveReceived(x => x.SaveChangesAsync(testContext.CancellationToken));
            }
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
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.ReadActiveIdAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsFailure.ShouldBeTrue();
                result.Error.Message.ShouldContain(userId.ToString());

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
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
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var result = await uut.ReadActiveIdAsync(
                    userId,
                    testContext.CancellationToken);

                result.IsSuccess.ShouldBeTrue();
                result.Value.ShouldBe(ticketId);

                testContext.MockContext.ShouldNotHaveReceived(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
            }
        }

        #endregion ReadActiveIdAsync() Tests

        #region ReadIdentitiesAsync() Tests

        internal static readonly IReadOnlyList<TestCaseData> ReadIdentitiesAsync_TestCaseData
            = new[]
            {
                /*                  isDeleted                           ticketIds                       */
                new TestCaseData(   Optional<bool>.Unspecified,         new[] { 1L, 2L, 3L, 4L, 5L })   .SetName("{m}()"),
                new TestCaseData(   Optional<bool>.FromValue(true),     new[] { 1L, 2L             })   .SetName("{m}(isDeleted: true)"),
                new TestCaseData(   Optional<bool>.FromValue(false),    new[] {         3L, 4L, 5L })   .SetName("{m}(isDeleted: false)")
            };

        [TestCaseSource(nameof(ReadIdentitiesAsync_TestCaseData))]
        public async Task ReadIdentitiesAsync_Always_ReturnsMatches(
            Optional<bool> isDeleted,
            IReadOnlyList<long> ticketIds)
        {
            using (var testContext = new TestContext())
            {
                var uut = testContext.BuildUut();

                var results = await uut.ReadIdentitiesAsync(
                    testContext.CancellationToken,
                    isDeleted);

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
        }

        #endregion ReadIdentitiesAsync() Tests
    }
}
