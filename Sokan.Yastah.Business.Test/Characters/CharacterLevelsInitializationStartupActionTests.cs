using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.Internal;

using Moq;
using NUnit.Framework;
using Shouldly;

using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Auditing;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;
using Microsoft.Extensions.DependencyInjection;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterLevelsInitializationStartupActionTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodWithLoggerTestContext
        {
            public TestContext()
            {
                UtcNow = default;
                NextAdministrationActionId = default;

                MockAuditableActionsRepository = new Mock<IAuditableActionsRepository>();
                MockAuditableActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong?>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockCharacterLevelsRepository = new Mock<ICharacterLevelsRepository>();

                MockServiceProvider = new Mock<IServiceProvider>();
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(IAuditableActionsRepository)))
                    .Returns(() => MockAuditableActionsRepository.Object);
                MockServiceProvider
                    .Setup(x => x.GetService(typeof(ICharacterLevelsRepository)))
                    .Returns(() => MockCharacterLevelsRepository.Object);

                MockServiceScope = new Mock<IServiceScope>();
                MockServiceScope
                    .Setup(x => x.ServiceProvider)
                    .Returns(() => MockServiceProvider.Object);

                MockServiceScopeFactory = new Mock<IServiceScopeFactory>();
                MockServiceScopeFactory
                    .Setup(x => x.CreateScope())
                    .Returns(() => MockServiceScope.Object);

                MockSystemClock = new Mock<ISystemClock>();
                MockSystemClock
                    .Setup(x => x.UtcNow)
                    .Returns(() => UtcNow);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);

                MockTransactionScope = new Mock<ITransactionScope>();

                MockYastahAutoMigrationStartupAction = new Mock<IYastahAutoMigrationStartupAction>();
                MockYastahAutoMigrationStartupAction
                    .Setup(x => x.WhenDone)
                    .Returns(_autoMigrationCompletionSource.Task);
            }

            public DateTimeOffset UtcNow;
            public long NextAdministrationActionId;

            public readonly Mock<IAuditableActionsRepository> MockAuditableActionsRepository;
            public readonly Mock<ICharacterLevelsRepository> MockCharacterLevelsRepository;
            public readonly Mock<IServiceProvider> MockServiceProvider;
            public readonly Mock<IServiceScope> MockServiceScope;
            public readonly Mock<IServiceScopeFactory> MockServiceScopeFactory;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;
            public readonly Mock<IYastahAutoMigrationStartupAction> MockYastahAutoMigrationStartupAction;

            public CharacterLevelsInitializationStartupAction BuildUut()
                => new CharacterLevelsInitializationStartupAction(
                    LoggerFactory.CreateLogger<CharacterLevelsInitializationStartupAction>(),
                    MockServiceScopeFactory.Object,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object,
                    MockYastahAutoMigrationStartupAction.Object);

            public void CompleteAutoMigration()
                => _autoMigrationCompletionSource.SetResult(null);

            public void SetIsLevel1ProperlyConfigured(
                    bool isLevel1ProperlyConfigured)
                => MockCharacterLevelsRepository
                    .Setup(x => x.AnyDefinitionsAsync(
                        It.IsAny<Optional<int>>(),
                        It.IsAny<Optional<int>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isLevel1ProperlyConfigured);

            private readonly TaskCompletionSource<object?> _autoMigrationCompletionSource
                = new TaskCompletionSource<object?>();
        }

        #endregion Test Context

        #region OnStartingAsync() Tests

        [Test]
        public async Task OnStartingAsync_Always_AwaitsAutoMigrationAction()
        {
            using var testContext = new TestContext();

            var uut = testContext.BuildUut();

            var result = uut.StartAsync(
                testContext.CancellationToken);

            result.IsCompleted.ShouldBeFalse();

            testContext.MockServiceProvider.Invocations.ShouldBeEmpty();
            testContext.MockTransactionScopeFactory.Invocations.ShouldBeEmpty();

            testContext.CompleteAutoMigration();

            await result;
        }

        [Test]
        public async Task OnStartingAsync_Level1IsProperlyConfigured_DoesNothing()
        {
            using var testContext = new TestContext();
            testContext.CompleteAutoMigration();
            testContext.SetIsLevel1ProperlyConfigured(true);

            var uut = testContext.BuildUut();

            await uut.StartAsync(
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .AnyDefinitionsAsync(
                    1,
                    0,
                    false,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldNotHaveReceived(x => x
                .CreateAsync(
                    It.IsAny<int>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<ulong?>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockCharacterLevelsRepository.ShouldNotHaveReceived(x => x
                .MergeDefinitionAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
        }

        public static readonly IReadOnlyList<TestCaseData> OnStartingAsync_Merge_TestCaseData
            = new[]
            {
                /*                  performed,                          actionId        */
                new TestCaseData(   default(DateTimeOffset),            default(long)   ).SetName("{m}(Default Values)"),
                new TestCaseData(   DateTimeOffset.MinValue,            long.MinValue   ).SetName("{m}(Min Values)"),
                new TestCaseData(   DateTimeOffset.MaxValue,            long.MaxValue   ).SetName("{m}(Max Values)"),
                new TestCaseData(   DateTimeOffset.Parse("2001-02-03"), 4L              ).SetName("{m}(Unique Value Set 1)"),
                new TestCaseData(   DateTimeOffset.Parse("2005-06-07"), 8L              ).SetName("{m}(Unique Value Set 2)"),
                new TestCaseData(   DateTimeOffset.Parse("2009-10-11"), 12L             ).SetName("{m}(Unique Value Set 3)")
            };

        [TestCaseSource(nameof(OnStartingAsync_Merge_TestCaseData))]
        public async Task OnStartingAsync_Level1IsNotProperlyConfigured_MergesLevel1Definition(
            DateTimeOffset performed,
            long actionId)
        {
            using var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };
            testContext.CompleteAutoMigration();
            testContext.SetIsLevel1ProperlyConfigured(false);

            var uut = testContext.BuildUut();

            await uut.StartAsync(
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .AnyDefinitionsAsync(
                    1,
                    0,
                    false,
                    testContext.CancellationToken));

            testContext.MockAuditableActionsRepository.ShouldHaveReceived(x => x
                .CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsInitialized,
                    performed,
                    null,
                    testContext.CancellationToken));

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .MergeDefinitionAsync(
                    1,
                    0,
                    false,
                    actionId,
                    testContext.CancellationToken));

            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Dispose());
            testContext.MockTransactionScope.ShouldHaveReceived(x => x
                .Complete());
        }

        #endregion OnStartingAsync() Tests
    }
}
