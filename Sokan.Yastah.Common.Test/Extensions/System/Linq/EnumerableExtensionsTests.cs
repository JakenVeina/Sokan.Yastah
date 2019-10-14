using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Moq;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System.Linq
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string[]> SequenceTestCases
            = new[]
            {
                Array.Empty<string>(),
                new[] { null as string },
                new[] { "A" },
                new[] { "A", "B", "C" }
            };

        #endregion Test Cases

        #region Do() Tests

        [TestCaseSource(nameof(SequenceTestCases))]
        public void Do_Always_InvokesActionForEachItemInSequence(params string[] sequence)
        {
            var mockAction = new Mock<Action<string>>();

            sequence.Do(mockAction.Object)
                .ToArray();

            mockAction.Invocations
                .Select(x => x.Arguments[0])
                .Cast<string>()
                .ShouldBeSequenceEqualTo(sequence);
        }

        [TestCaseSource(nameof(SequenceTestCases))]
        public void Do_Always_ReturnsEachItemInSequence(params string[] sequence)
        {
            var mockAction = new Mock<Action<string>>();

            var result = sequence.Do(mockAction.Object)
                .ToArray();

            result.ShouldBeSequenceEqualTo(sequence);
        }

        #endregion Do() Tests

        #region ForEach() Tests

        [TestCaseSource(nameof(SequenceTestCases))]
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
