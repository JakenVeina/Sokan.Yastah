using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.Messaging;

namespace Sokan.Yastah.Common.Test.Messaging
{
    [TestFixture]
    public class MessengerTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<TestCaseData> NotificationHandlerCount_TestCaseData
            = new[]
            {
                /*                  notificationHandlerCount    */
                new TestCaseData(   0                           ).SetName("{m}(0 Notification Handlers)"),
                new TestCaseData(   1                           ).SetName("{m}(1 Notification Handler)"),
                new TestCaseData(   5                           ).SetName("{m}(5 Notification Handlers)")
            };

        internal class TestContext
            : AsyncMethodWithLoggerTestContext
        {
            public TestContext()
            {
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<object>>)))
                    .Returns(MockNotificationHandlers.Select(x => x.Object));
            }

            public readonly Mock<IServiceProvider>                      MockServiceProvider         = new Mock<IServiceProvider>();
            public readonly List<Mock<INotificationHandler<object>>>    MockNotificationHandlers    = new List<Mock<INotificationHandler<object>>>();

            public Messenger BuildUut()
                => new Messenger(
                    LoggerFactory.CreateLogger<Messenger>(),
                    MockServiceProvider.Object);
        }

        #endregion Test Cases

        #region PublishNotificationAsync() Tests

        [TestCaseSource(nameof(NotificationHandlerCount_TestCaseData))]
        public async Task PublishNotificationAsync_Always_PublishesNotificationToEachHandler(
            int notificationHandlerCount)
        {
            using var testContext = new TestContext();

            foreach (var _ in Enumerable.Repeat(0, notificationHandlerCount))
                testContext.MockNotificationHandlers.Add(new Mock<INotificationHandler<object>>());

            var uut = testContext.BuildUut();

            var notification = new object();

            await uut.PublishNotificationAsync(
                notification,
                testContext.CancellationToken);

            testContext.MockNotificationHandlers
                .ForEach(nh => nh.ShouldHaveReceived(x => x.OnNotificationPublishedAsync(notification, testContext.CancellationToken)));
        }

        #endregion PublishNotificationAsync() Tests
    }
}
