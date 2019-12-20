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

        public static readonly IReadOnlyList<string?> Value_TestCaseData
            = new[]
            {
                null,
                "Value"
            };

        public static readonly IReadOnlyList<TestCaseData> OperationResult_TestCaseData
            = new[]
            {
                /*                  value,      error           */
                new TestCaseData(   "value",    null            ).SetName("{m}(Success)"),
                new TestCaseData(   null,       MakeFakeError() ).SetName("{m}(Failure)")
            };

        public static readonly IReadOnlyList<TestCaseData> OperationResultsAreNotEqual_TestCaseData
            = new[]
            {
                /*                  xValue,     xError,             yValue,     yError          */
                new TestCaseData(   "value 1",  null,               "value 2",  MakeFakeError() ).SetName("{m}(Both Success, Different Value)"),
                new TestCaseData(   null,       MakeFakeError(),    null,       MakeFakeError() ).SetName("{m}(Both Failure, Different Error)"),
                new TestCaseData(   "value 3",  null,               null,       MakeFakeError() ).SetName("{m}(Success, Failure)"),
                new TestCaseData(   null,       MakeFakeError(),    "value 4",  null            ).SetName("{m}(Failure, Success)"),
            };

        private static IOperationError MakeFakeError()
            => new Mock<IOperationError>().Object;

        #endregion Test Cases

        #region Default Tests

        [Test]
        public void Default_Always_IsSuccess()
        {
            var result = default(OperationResult<string?>);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [Test]
        public void Default_Always_ValueIsDefault()
        {
            default(OperationResult<string?>)
                .Value.ShouldBe(default);
        }

        [Test]
        public void Default_Always_ErrorThrowsException()
        {
            Should.Throw<InvalidOperationException>(() =>
            {
                _ = default(OperationResult<string?>).Error;
            });
        }

        #endregion Default Tests

        #region FromError() Tests

        [Test]
        public void FromError_Otherwise_IsFailure()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string?>.FromError(error);

            result.IsSuccess.ShouldBeFalse();
            result.IsFailure.ShouldBeTrue();
        }

        [Test]
        public void FromError_Otherwise_ErrorIsGiven()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string?>.FromError(error);

            result.Error.ShouldBeSameAs(error);
        }

        [Test]
        public void FromError_Otherwise_ValueThrowsException()
        {
            var error = new Mock<IOperationError>().Object;

            var result = OperationResult<string?>.FromError(error);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Value;
            });
        }

        #endregion FromError() Tests

        #region FromValue() Tests

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void FromValue_Always_IsSuccess(
            string? value)
        {
            var result = OperationResult<string?>.FromValue(value);

            result.IsSuccess.ShouldBeTrue();
            result.IsFailure.ShouldBeFalse();
        }

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void FromValue_Always_ErrorThrowsException(
            string? value)
        {
            var result = OperationResult<string?>.FromValue(value);

            Should.Throw<InvalidOperationException>(() =>
            {
                _ = result.Error;
            });
        }

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void FromValue_Always_ValueIsGiven(
            string? value)
        {
            var result = OperationResult<string?>.FromValue(value);

            result.Value.ShouldBe(value);
        }

        #endregion FromValue() Tests

        #region (OperationResult)OperationResult<T> Tests

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void Operator_CastToOperationResult_OperationResultIsSuccess_ResultIsSuccess(
            string? value)
        {
            var operationResult = OperationResult.FromValue(value);

            var result = (OperationResult)operationResult;

            result.IsSuccess.ShouldBeTrue();
        }

        [Test]
        public void Operator_CastToOperationResult_OperationResultIsFailure_ResultIsFromError()
        {
            var error = new Mock<IOperationError>().Object;

            var operationResult = OperationResult.FromError<string?>(error);

            var result = (OperationResult)operationResult;

            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBeSameAs(error);
        }

        #endregion (OperationResult)OperationResult<T> Tests

        #region (OperationResult<T>)T Tests

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void Operator_CastFromValue_Always_ResultIsFromValue(
            string? value)
        {
            var result = (OperationResult<string?>)value;

            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(value);
        }

        #endregion (OperationResult<T>)T Tests

        #region Equals() Tests

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNull_ReturnsFalse(
            string? value,
            IOperationError? error)
        {
            var uut = (error is null)
                ? OperationResult<string?>.FromValue(value)
                : OperationResult<string?>.FromError(error);

            uut.Equals(null as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotOperationResult_ReturnsFalse(
            string? value,
            IOperationError? error)
        {
            var uut = (error is null)
                ? OperationResult<string?>.FromValue(value)
                : OperationResult<string?>.FromError(error);

            uut.Equals("obj" as object).ShouldBeFalse();
        }

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Equals_NonGeneric_ObjIsEqual_ReturnsTrue(
            string? value,
            IOperationError? error)
        {
            var (uut, obj) = (error is null)
                ? (OperationResult<string?>.FromValue(value),   OperationResult<string?>.FromValue(value) as object)
                : (OperationResult<string?>.FromError(error),    OperationResult<string?>.FromError(error) as object);

            uut.Equals(obj).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OperationResultsAreNotEqual_TestCaseData))]
        public void Equals_NonGeneric_ObjIsNotEqual_ReturnsFalse(
            string? xValue,
            IOperationError? xError,
            string? yValue,
            IOperationError? yError)
        {
            var uut = (xError is null)
                ? OperationResult<string?>.FromValue(xValue)
                : OperationResult<string?>.FromError(xError);

            var obj = (yError is null)
                    ? OperationResult<string?>.FromValue(yValue)
                    : OperationResult<string?>.FromError(yError)
                as object;

            uut.Equals(obj).ShouldBeFalse();
        }

        #endregion Equals() Tests

        #region Equals<T>() Tests

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Equals_Generic_OtherIsEqual_ReturnsTrue(
            string? value,
            IOperationError? error)
        {
            var (uut, other) = (error is null)
                ? (OperationResult<string?>.FromValue(value),   OperationResult<string?>.FromValue(value) as object)
                : (OperationResult<string?>.FromError(error),    OperationResult<string?>.FromError(error) as object);

            uut.Equals(other).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OperationResultsAreNotEqual_TestCaseData))]
        public void Equals_Generic_OtherIsNotEqual_ReturnsFalse(
            string? xValue,
            IOperationError? xError,
            string? yValue,
            IOperationError? yError)
        {
            var uut = (xError is null)
                ? OperationResult<string?>.FromValue(xValue)
                : OperationResult<string?>.FromError(xError);

            var other = (yError is null)
                ? OperationResult<string?>.FromValue(yValue)
                : OperationResult<string?>.FromError(yError);

            uut.Equals(other).ShouldBeFalse();
        }

        #endregion Equals<T>() Tests

        #region GetHashCode() Tests

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void GetHashCode_UnitsAreEqual_HashCodesAreEqual(
            string? value,
            IOperationError? error)
        {
            var (uut, other) = (error is null)
                ? (OperationResult<string?>.FromValue(value),   OperationResult<string?>.FromValue(value))
                : (OperationResult<string?>.FromError(error),    OperationResult<string?>.FromError(error));

            uut.GetHashCode().ShouldBe(other.GetHashCode());
        }

        [TestCaseSource(nameof(OperationResultsAreNotEqual_TestCaseData))]
        public void GetHashCode_UnitsAreNotEqual_HashCodesAreNotEqual(
            string? xValue,
            IOperationError? xError,
            string? yValue,
            IOperationError? yError)
        {
            var uut = (xError is null)
                ? OperationResult<string?>.FromValue(xValue)
                : OperationResult<string?>.FromError(xError);

            var other = (yError is null)
                ? OperationResult<string?>.FromValue(yValue)
                : OperationResult<string?>.FromError(yError);

            uut.GetHashCode().ShouldNotBe(other.GetHashCode());
        }

        #endregion GetHashCode() Tests

        #region ToString() Tests

        [TestCaseSource(nameof(Value_TestCaseData))]
        public void ToString_UnitIsSuccess_ResultContainsValue(
            string? value)
        {
            var result = OperationResult<string?>.FromValue(value)
                .ToString();

            if (value is null)
                result.ShouldContain("null");
            else
                result.ShouldContain(value);
        }

        [Test]
        public void ToString_UnitIsFailure_ResultContainsError()
        {
            var mockError = new Mock<IOperationError>();

            var errorToString = nameof(mockError);
            mockError
                .Setup(x => x.ToString())
                .Returns(errorToString);

            var result = OperationResult<string?>.FromError(mockError.Object)
                .ToString();

            mockError.ShouldHaveReceived(x => x.ToString());

            result.ShouldContain(errorToString);

        }

        #endregion ToString() Tests

        #region == Tests

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Operator_Equals_XAndYAreEqual_ReturnsTrue(
            string? value,
            IOperationError? error)
        {
            var (x, y) = (error is null)
                ? (OperationResult<string?>.FromValue(value),   OperationResult<string?>.FromValue(value))
                : (OperationResult<string?>.FromError(error),    OperationResult<string?>.FromError(error));

            (x == y).ShouldBeTrue();
        }

        [TestCaseSource(nameof(OperationResultsAreNotEqual_TestCaseData))]
        public void Operator_Equals_XAndYAreNotEqual_ReturnsFalse(
            string? xValue,
            IOperationError? xError,
            string? yValue,
            IOperationError? yError)
        {
            var x = (xError is null)
                ? OperationResult<string?>.FromValue(xValue)
                : OperationResult<string?>.FromError(xError);

            var y = (yError is null)
                ? OperationResult<string?>.FromValue(yValue)
                : OperationResult<string?>.FromError(yError);

            (x == y).ShouldBeFalse();
        }

        #endregion == Tests

        #region != Tests

        [TestCaseSource(nameof(OperationResult_TestCaseData))]
        public void Operator_NotEquals_XAndYAreEqual_ReturnsFalse(
            string? value,
            IOperationError? error)
        {
            var (x, y) = (error is null)
                ? (OperationResult<string?>.FromValue(value),   OperationResult<string?>.FromValue(value))
                : (OperationResult<string?>.FromError(error),    OperationResult<string?>.FromError(error));
            
            (x != y).ShouldBeFalse();
        }

        [TestCaseSource(nameof(OperationResultsAreNotEqual_TestCaseData))]
        public void Operator_NotEquals_XAndYAreNotEqual_ReturnsTrue(
            string? xValue,
            IOperationError? xError,
            string? yValue,
            IOperationError? yError)
        {
            var x = (xError is null)
                ? OperationResult<string?>.FromValue(xValue)
                : OperationResult<string?>.FromError(xError);

            var y = (yError is null)
                ? OperationResult<string?>.FromValue(yValue)
                : OperationResult<string?>.FromError(yError);

            (x != y).ShouldBeTrue();
        }

        #endregion != Tests
    }
}
