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

        public static readonly IReadOnlyList<TestCaseData> TestContextTestCaseData
            = new[]
            {
                new TestCaseData(new TestContext(0))
                    .SetName("{m}: 0 Notification Handlers"),
                new TestCaseData(new TestContext(1))
                    .SetName("{m}: 1 Notification Handler"),
                new TestCaseData(new TestContext(5))
                    .SetName("{m}: 5 Notification Handlers")
            };

        public class TestContext
            : AsyncMethodTestContextBase
        {
            public TestContext(int notificationHandlerCount)
            {
                Enumerable.Range(0, notificationHandlerCount)
                    .Select(x => new Mock<INotificationHandler<object>>())
                    .ForEach(x => _mockNotificationHandlers.Add(x));

                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IEnumerable<INotificationHandler<object>>)))
                    .Returns(_mockNotificationHandlers.Select(x => x.Object));

            }

            public Mock<IServiceProvider> MockServiceProvider { get; }
                = new Mock<IServiceProvider>();

            public IReadOnlyList<Mock<INotificationHandler<object>>> MockNotificationHandlers
                => _mockNotificationHandlers;
            private readonly List<Mock<INotificationHandler<object>>> _mockNotificationHandlers
                = new List<Mock<INotificationHandler<object>>>();

            public Messenger BuildUut()
                => new Messenger(
                    MockServiceProvider.Object);
        }

        #endregion Test Cases

            #region PublishNotificationAsync() Tests

        [TestCaseSource(nameof(TestContextTestCaseData))]
        public async Task PublishNotificationAsync_Always_PublishesNotificationToEachHandler(
            TestContext testContext)
        {
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
