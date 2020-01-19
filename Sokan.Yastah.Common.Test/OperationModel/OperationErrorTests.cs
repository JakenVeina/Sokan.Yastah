using System.Collections.Generic;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class OperationErrorTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string> ValidMessageTestCases
            = new[]
            {
                "",
                "This is a message",
                "This is only a message"
            };

        internal static Mock<OperationError> BuildMockUut(string message)
            => new Mock<OperationError>(message)
            {
                CallBase = true
            };

        #endregion Test Cases

        #region Constructor Tests

        [TestCaseSource(nameof(ValidMessageTestCases))]
        public void Constructor_Always_CodeIsTypeName(
            string message)
        {
            var uut = BuildMockUut(message).Object;

            uut.Code.ShouldBe(uut.GetType().Name);
        }

        [TestCaseSource(nameof(ValidMessageTestCases))]
        public void Constructor_Always_MessageIsGiven(
            string message)
        {
            var uut = BuildMockUut(message).Object;

            uut.Message.ShouldBe(message);
        }

        #endregion Constructor Tests

        #region ToString() Tests

        [TestCaseSource(nameof(ValidMessageTestCases))]
        public void ToString_Always_ResultContainsCodeAndMessage(
            string message)
        {
            var uut = BuildMockUut(message).Object;

            var result = uut.ToString();

            result.ShouldContain(uut.Code);
            result.ShouldContain(uut.Message);
        }

        #endregion ToString() Tests
    }
}
