using System;
using System.Collections.Generic;

using Moq;
using NUnit.Framework;
using Shouldly;

namespace Sokan.Yastah.Common.Test.Extensions.System
{
    [TestFixture]
    public class LazyExTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<TestCaseData> Create_TestCaseData
            = new[]
            {
                new TestCaseData(   default(string) ),
                new TestCaseData(   string.Empty    ),
                new TestCaseData(   "1"             ),
                new TestCaseData(   "2"             ),
                new TestCaseData(   "3"             )
            };

        #endregion Test Cases

        #region Create() Tests

        [TestCaseSource(nameof(Create_TestCaseData))]
        public void Create_Always_ReturnsNewLazy(
            string value)
        {
            var mockValueFactory = new Mock<Func<string>>();
            mockValueFactory
                .Setup(x => x())
                .Returns(value);

            var result = LazyEx.Create(
                mockValueFactory.Object);

            result.IsValueCreated.ShouldBeFalse();
            result.Value.ShouldBe(value);

            mockValueFactory.ShouldHaveReceived(x => x());            
        }

        #endregion Create() Tests

        #region CreateThreadSafe() Tests

        [TestCaseSource(nameof(Create_TestCaseData))]
        public void CreateThreadSafe_Always_ReturnsNewLazy(
            string value)
        {
            var mockValueFactory = new Mock<Func<string>>();
            mockValueFactory
                .Setup(x => x())
                .Returns(value);

            var result = LazyEx.CreateThreadSafe(
                mockValueFactory.Object);

            result.IsValueCreated.ShouldBeFalse();
            result.Value.ShouldBe(value);

            mockValueFactory.ShouldHaveReceived(x => x());
        }

        #endregion CreateThreadSafe() Tests
    }
}
