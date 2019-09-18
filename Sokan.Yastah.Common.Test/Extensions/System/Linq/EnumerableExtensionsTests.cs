using System;
using System.Linq;

using NUnit.Framework;
using Moq;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System.Linq
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        #region ForEach() Tests

        [TestCase()]
        [TestCase(null)]
        [TestCase("A")]
        [TestCase("A", "B", "C")]
        public void ForEach_Always_InvokesActionForEachItemInSequence(params string[] sequence)
        {
            var mockAction = new Mock<Action<string>>();

            sequence.ForEach(mockAction.Object);

            mockAction.Invocations
                .Select(x => x.Arguments[0])
                .Cast<string>()
                .ShouldBeSequenceEqualTo(sequence);
        }

        #endregion ForEach() Tests
    }
}
