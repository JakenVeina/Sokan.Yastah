using System;

using NUnit.Framework;
using Shouldly;

using System.Transactions;

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
            var uut = new TransactionScopeFactory();

            using (var result = uut.CreateScope(isolationLevel))
            {
                result.Options.IsolationLevel.ShouldBe(expectedIsolationLevel);
                result.Options.Timeout.ShouldBe(TimeSpan.FromSeconds(30));
            }
        }

        [Test]
        public void CreateScope_ResultHasBeenDisposed_CompleteThrowsException()
        {
            var uut = new TransactionScopeFactory();

            var result = uut.CreateScope();

            result.Dispose();

            Should.Throw<ObjectDisposedException>(() =>
            {
                result.Complete();
            });
        }

        [Test]
        public void CreateScope_InnerTransactionWasNotCompleted_DisposeThrowsException()
        {
            var uut = new TransactionScopeFactory();

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
