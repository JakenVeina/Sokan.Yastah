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
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Business.Test.Characters
{
    [TestFixture]
    public class CharacterLevelsInitializationBehaviorTests
    {
        #region Test Context

        internal class TestContext
            : AsyncMethodTestContext
        {
            public TestContext()
            {
                UtcNow = default;
                NextAdministrationActionId = default;

                MockAdministrationActionsRepository = new Mock<IAdministrationActionsRepository>();
                MockAdministrationActionsRepository
                    .Setup(x => x.CreateAsync(
                        It.IsAny<int>(),
                        It.IsAny<DateTimeOffset>(),
                        It.IsAny<ulong?>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => NextAdministrationActionId);

                MockCharacterLevelsRepository = new Mock<ICharacterLevelsRepository>();

                MockSystemClock = new Mock<ISystemClock>();
                MockSystemClock
                    .Setup(x => x.UtcNow)
                    .Returns(() => UtcNow);

                MockTransactionScopeFactory = new Mock<ITransactionScopeFactory>();
                MockTransactionScopeFactory
                    .Setup(x => x.CreateScope(It.IsAny<IsolationLevel?>()))
                    .Returns(() => MockTransactionScope.Object);

                MockTransactionScope = new Mock<ITransactionScope>();
            }

            public DateTimeOffset UtcNow;
            public long NextAdministrationActionId;

            public readonly Mock<IAdministrationActionsRepository> MockAdministrationActionsRepository;
            public readonly Mock<ICharacterLevelsRepository> MockCharacterLevelsRepository;
            public readonly Mock<ISystemClock> MockSystemClock;
            public readonly Mock<ITransactionScopeFactory> MockTransactionScopeFactory;
            public readonly Mock<ITransactionScope> MockTransactionScope;

            public CharacterLevelsInitializationBehavior BuildUut()
                => new CharacterLevelsInitializationBehavior(
                    MockAdministrationActionsRepository.Object,
                    MockCharacterLevelsRepository.Object,
                    MockSystemClock.Object,
                    MockTransactionScopeFactory.Object);

            public void SetIsLevel1ProperlyConfigured(
                    bool isLevel1ProperlyConfigured)
                => MockCharacterLevelsRepository
                    .Setup(x => x.AnyAsync(
                        It.IsAny<Optional<int>>(),
                        It.IsAny<Optional<int>>(),
                        It.IsAny<Optional<bool>>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(isLevel1ProperlyConfigured);
        }

        #endregion Test Context

        #region OnStartupAsync() Tests

        [Test]
        public async Task OnStartupAsync_Level1IsProperlyConfigured_DoesNothing()
        {
            var testContext = new TestContext();

            testContext.SetIsLevel1ProperlyConfigured(true);

            var uut = testContext.BuildUut();

            await uut.OnStartupAsync(
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .AnyAsync(
                    1,
                    0,
                    false,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldNotHaveReceived(x => x
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

        public static readonly IReadOnlyList<TestCaseData> OnStartupAsync_Merge_TestCaseData
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

        [TestCaseSource(nameof(OnStartupAsync_Merge_TestCaseData))]
        public async Task OnStartupAsync_Level1IsNotProperlyConfigured_MergesLevel1Definition(
            DateTimeOffset performed,
            long actionId)
        {
            var testContext = new TestContext()
            {
                UtcNow = performed,
                NextAdministrationActionId = actionId
            };

            testContext.SetIsLevel1ProperlyConfigured(false);

            var uut = testContext.BuildUut();

            await uut.OnStartupAsync(
                testContext.CancellationToken);

            testContext.MockTransactionScopeFactory.ShouldHaveReceived(x => x
                .CreateScope(default));

            testContext.MockCharacterLevelsRepository.ShouldHaveReceived(x => x
                .AnyAsync(
                    1,
                    0,
                    false,
                    testContext.CancellationToken));

            testContext.MockAdministrationActionsRepository.ShouldHaveReceived(x => x
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

        #endregion OnStartupAsync() Tests
    }
}
