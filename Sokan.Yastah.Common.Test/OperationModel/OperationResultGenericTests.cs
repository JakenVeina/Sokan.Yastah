using System;
using System.Collections.Generic;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class OperationResultGenericTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<string> ValueTestCases
            = new[]
            {
                null,
                "Value"
            };

        #endregion Test Cases

        #region Default Tests

        [Test]
        public void Default_Always_IsSuccess()
        {
            var result = default(OperationResult<string>);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [Test]
        public void Default_Always_ValueIsDefault()
        {
            default(OperationResult<string>)
                .Value.ShouldBe(default);
        }

        [Test]
        public void Default_Always_ErrorThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = default(OperationResult<string>).Error;
            });
        }

        #endregion Default Tests

        #region FromError() Tests

        [Test]
        public void FromError_ErrorIsNull_ThrowsException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = OperationResult<string>.FromError(null);
            });
        }

        [Test]
        public void FromError_Otherwise_IsFailure()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string>.FromError(error);

            result.IsSuccess.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
        }

        [Test]
        public void FromError_Otherwise_ErrorIsGiven()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string>.FromError(error);

            result.Error.ShouldBeSameAs(error);
        }

        [Test]
        public void FromError_Otherwise_ValueThrowsException()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string>.FromError(error);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Value;
            });
        }

        #endregion FromError() Tests

        #region FromValue() Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_IsSuccess(
            string value)
        {
            var result = OperationResult<string>.FromValue(value);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_ErrorThrowsException(
            string value)
        {
            var result = OperationResult<string>.FromValue(value);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Error;
            });
        }

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_ValueIsGiven(
            string value)
        {
            var result = OperationResult<string>.FromValue(value);

            result.Value.ShouldBe(value);
        }

        #endregion FromValue() Tests

        #region (OperationResult)OperationResult<T> Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void Operator_CastToOperationResult_OperationResultIsSuccess_ResultIsSuccess(
            string value)
        {
            var operationResult = OperationResult.FromValue(value);

            var result = (OperationResult)operationResult;

            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void Operator_CastToOperationResult_OperationResultIsFailure_ResultIsFromError()
        {
            var error = new Mock<IOperationError>().Object;

            var operationResult = OperationResult.FromError<string>(error);

            var result = (OperationResult)operationResult;

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(error);
        }

        #endregion (OperationResult)OperationResult<T> Tests

        #region (OperationResult<T>)T Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void Operator_CastFromValue_Always_ResultIsFromValue(
            string value)
        {
            var result = (OperationResult<string>)value;

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion (OperationResult<T>)T Tests
    }
}
