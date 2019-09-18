using System;
using System.Collections.Generic;

using NUnit.Framework;
using Moq;
using Shouldly;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Common.Test.OperationModel
{
    [TestFixture]
    public class OperationResultTests
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
            var result = default(OperationResult);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [Test]
        public void Default_Always_ErrorThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = default(OperationResult).Error;
            });
        }

        #endregion Default Tests

        #region Success Tests

        [Test]
        public void Success_Always_IsSuccess()
        {
            var result = OperationResult.Success;

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [Test]
        public void Success_Always_ErrorThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = OperationResult.Success.Error;
            });
        }

        #endregion Success Tests

        #region FromError() Tests

        [Test]
        public void FromError_ErrorIsNull_ThrowsException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = OperationResult.FromError(null);
            });
        }

        [Test]
        public void FromError_Otherwise_IsFailure()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult.FromError(error);

            result.IsSuccess.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
        }

        [Test]
        public void FromError_Otherwise_ErrorIsGiven()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult.FromError(error);

            result.Error.ShouldBeSameAs(error);
        }

        #endregion FromError() Tests

        #region FromError<T>() Tests

        [Test]
        public void FromError_Generic_ErrorIsNull_ThrowsException()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                _ = OperationResult.FromError<string>(null);
            });
        }

        [Test]
        public void FromError_Generic_Otherwise_IsFailure()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult.FromError<string>(error);

            result.IsSuccess.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
        }

        [Test]
        public void FromError_Generic_Otherwise_ErrorIsGiven()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult.FromError<string>(error);

            result.Error.ShouldBeSameAs(error);
        }

        [Test]
        public void FromError_Generic_Otherwise_ValueThrowsException()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult.FromError<string>(error);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Value;
            });
        }

        #endregion FromError<T>() Tests

        #region FromValue<T>() Tests

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_IsSuccess(
            string value)
        {
            var result = OperationResult.FromValue(value);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_ErrorThrowsException(
            string value)
        {
            var result = OperationResult.FromValue(value);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Error;
            });
        }

        [TestCaseSource(nameof(ValueTestCases))]
        public void FromValue_Always_ValueIsGiven(
            string value)
        {
            var result = OperationResult.FromValue(value);

            result.Value.ShouldBe(value);
        }

        #endregion FromValue<T>() Tests
    }
}
