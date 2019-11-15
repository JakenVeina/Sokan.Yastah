using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class OperationErrorBaseTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string> ValidMessageTestCases
            = new[]
            {
                "",
                "This is a message",
                "This is only a message"
            };

        #endregion Test Cases

        #region Constructor Tests

        [TestCaseSource(nameof(ValidMessageTestCases))]
        public void Constructor_Always_CodeIsTypeName(
            string message)
        {
            var uut = new Mock<OperationErrorBase>(message).Object;

            uut.Code.ShouldBe(uut.GetType().Name);
        }

        [TestCaseSource(nameof(ValidMessageTestCases))]
        public void Constructor_Always_MessageIsGiven(
            string message)
        {
            var uut = new Mock<OperationErrorBase>(message).Object;

            uut.Message.ShouldBe(message);
        }

        #endregion Constructor Tests
    }
}
