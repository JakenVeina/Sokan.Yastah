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

        public static readonly IReadOnlyList<string?[]> SequenceTestCases
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
        public void Do_Always_InvokesActionForEachItemInSequence(params string?[] sequence)
        {
            var mockAction = new Mock<Action<string?>>();

            sequence.Do(mockAction.Object)
                .ToArray();

            mockAction.Invocations
                .Select(x => x.Arguments[0])
                .Cast<string>()
                .ShouldBeSequenceEqualTo(sequence);
        }

        [TestCaseSource(nameof(SequenceTestCases))]
        public void Do_Always_ReturnsEachItemInSequence(params string?[] sequence)
        {
            var mockAction = new Mock<Action<string?>>();

            var result = sequence.Do(mockAction.Object)
                .ToArray();

            result.ShouldBeSequenceEqualTo(sequence);
        }

        #endregion Do() Tests

        #region ForEach() Tests

        [TestCaseSource(nameof(SequenceTestCases))]
        public void ForEach_Always_InvokesActionForEachItemInSequence(params string?[] sequence)
        {
            var mockAction = new Mock<Action<string?>>();

            sequence.ForEach(mockAction.Object);

            mockAction.Invocations
                .Select(x => x.Arguments[0])
                .Cast<string>()
                .ShouldBeSequenceEqualTo(sequence);
        }

        #endregion ForEach() Tests

        #region SetEquals() Tests

        public static readonly IReadOnlyList<TestCaseData> SetEquals_SequencesAreSetEqual_TestCaseData
            = new[]
            {
                /*                  first,                  second                  */
                new TestCaseData(   Array.Empty<int>(),     Array.Empty<int>()      ),
                new TestCaseData(   new[] { 1 },            new[] { 1 }             ),
                new TestCaseData(   new[] { 2, 3 },         new[] { 2, 3 }          ),
                new TestCaseData(   new[] { 4, 5 },         new[] { 5, 4 }          ),
                new TestCaseData(   new[] { 6, 6, 7 },      new[] { 6, 6, 7 }       ),
                new TestCaseData(   new[] { 8, 8, 9 },      new[] { 8, 9, 8 }       ),
                new TestCaseData(   new[] { 10, 10, 11 },   new[] { 11, 10, 10 }    )
            };

        [TestCaseSource(nameof(SetEquals_SequencesAreSetEqual_TestCaseData))]
        public void SetEquals_SequencesAreSetEqual_ReturnsTrue(
                IEnumerable<int> first,
                IEnumerable<int> second)
            => first.SetEquals(second).ShouldBeTrue();

        public static readonly IReadOnlyList<TestCaseData> SetEquals_SequencesAreNotSetEqual_TestCaseData
            = new[]
            {
                /*                  first,                  second                  */
                new TestCaseData(   Array.Empty<int>(),     new[] { 1 }             ),
                new TestCaseData(   new[] { 2 },            Array.Empty<int>()      ),
                new TestCaseData(   new[] { 3 },            new[] { 4 }             ),
                new TestCaseData(   new[] { 5, 5 },         new[] { 5 }             ),
                new TestCaseData(   new[] { 6, 7 },         new[] { 8, 9 }          ),
                new TestCaseData(   new[] { 10, 10, 11 },   new[] { 10, 11 }        )
            };

        [TestCaseSource(nameof(SetEquals_SequencesAreNotSetEqual_TestCaseData))]
        public void SetEquals_SequencesAreNotSetEqual_ReturnsFalse(
                IEnumerable<int> first,
                IEnumerable<int> second)
            => first.SetEquals(second).ShouldBeFalse();

        #endregion SetEquals() Tests
    }
}
