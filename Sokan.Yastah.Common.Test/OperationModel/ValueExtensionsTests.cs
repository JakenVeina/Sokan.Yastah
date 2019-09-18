using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class ValueExtensionsTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string> ValueTestCases
            = new[]
            {
                null,
                "Value"
            };

        #endregion Test Cases

        #region ToError() Tests

        [Test]
        public void ToError_Always_ResultIsFromError()
        {
            var error = new Mock<IOperationError>().Object;

            var result = error.ToError();

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(error);
        }

        #endregion ToError() Tests

        #region ToError<T>() Tests

        [Test]
        public void ToError_Generic_Always_ResultIsFromError()
        {
            var error = new Mock<IOperationError>().Object;

            var result = error.ToError<string>();

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(error);
        }

        #endregion ToError<T>() Tests

        #region ToOptional<T>() Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void ToOperator_Always_ResultIsFromValue(
            string value)
        {
            var result = value.ToOptional();

            result.IsSpecified.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion ToOptional<T>() Tests

        #region ToSuccess<T>() Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void ToSuccess_Always_ResultIsFromValue(
            string value)
        {
            var result = value.ToSuccess();

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion ToSuccess<T>() Tests
    }
}
