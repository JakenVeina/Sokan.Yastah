using System;

using NUnit.Framework;
using Shouldly;

using System.Transactions;

using Sokan.Yastah.Common.Test;

namespace Sokan.Yastah.Data.Test.Extensions.System.Transactions
{
    [TestFixture]
    [NonParallelizable]
    public class TransactionScopeFactoryTests
    {
        #region CreateScope() Tests

        [TestCase(IsolationLevel.Serializable,      IsolationLevel.Serializable)]
        [TestCase(IsolationLevel.RepeatableRead,    IsolationLevel.RepeatableRead)]
        [TestCase(IsolationLevel.ReadCommitted,     IsolationLevel.ReadCommitted)]
        [TestCase(IsolationLevel.ReadUncommitted,   IsolationLevel.ReadUncommitted)]
        [TestCase(IsolationLevel.Snapshot,          IsolationLevel.Snapshot)]
        [TestCase(IsolationLevel.Chaos,             IsolationLevel.Chaos)]
        [TestCase(IsolationLevel.Unspecified,       IsolationLevel.Unspecified)]
        [TestCase(null,                             IsolationLevel.ReadCommitted)]
        public void CreateScope_Always_ResultOptionsAreExpected(
            IsolationLevel? isolationLevel,
            IsolationLevel expectedIsolationLevel)
        {
            using var loggerFactory = new TestLoggerFactory();

            var uut = new TransactionScopeFactory(
                loggerFactory.CreateLogger<TransactionScopeFactory>());

            using var result = uut.CreateScope(isolationLevel);
            
            result.Options.IsolationLevel.ShouldBe(expectedIsolationLevel);
            result.Options.Timeout.ShouldBe(TimeSpan.FromSeconds(30));
        }

        [Test]
        public void CreateScope_ResultHasBeenDisposed_CompleteThrowsException()
        {
            using var loggerFactory = new TestLoggerFactory();

            var uut = new TransactionScopeFactory(
                loggerFactory.CreateLogger<TransactionScopeFactory>());

            var result = uut.CreateScope();

            #pragma warning disable IDISP017 // Prefer using.
            #pragma warning disable IDISP016 // Don't use disposed instance.
            result.Dispose();
            #pragma warning restore IDISP016 // Don't use disposed instance.
            #pragma warning restore IDISP017 // Prefer using.

            Should.Throw<ObjectDisposedException>(() =>
            {
                result.Complete();
            });
        }

        [Test]
        public void CreateScope_InnerTransactionWasNotCompleted_DisposeThrowsException()
        {
            using var loggerFactory = new TestLoggerFactory();

            var uut = new TransactionScopeFactory(
                loggerFactory.CreateLogger<TransactionScopeFactory>());

            var outerTransaction = uut.CreateScope();

            using (var innerTransaction = uut.CreateScope()) { }

            outerTransaction.Complete();

            Should.Throw<TransactionAbortedException>(() =>
            {
                outerTransaction.Dispose();
            });
        }

        #endregion CreateScope() Tests
    }
}
