using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;
using Shouldly;

using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Sokan.Yastah.Common.Test.Extensions.System
{
    [TestFixture]
    public class EnumExTests
    {
        #region Test Cases

        public static readonly IReadOnlyList<TestCaseData> TEnumTestCaseData
            = new[]
            {
                new TestCaseData(typeof(TestEnum1))
                    .SetName("{m}: Enum with no values"),
                new TestCaseData(typeof(TestEnum2))
                    .SetName("{m}: Enum with single value"),
                new TestCaseData(typeof(TestEnum3))
                    .SetName("{m}: Enum with many values"),
                new TestCaseData(typeof(TestEnum4))
                    .SetName("{m}: Enum with missing Description")
            };

        public enum TestEnum1 { }

        public enum TestEnum2
        {
            [Description("Value 1")] Value1
        }

        public enum TestEnum3
        {
            [Description("Value 1")] Value1,
            [Description("Value 2")] Value2,
            [Description("Value 3")] Value3
        }

        public enum TestEnum4
        {
            [Description("Value 1")] Value1,
                                     Value2,
            [Description("Value 3")] Value3
        }

        #endregion Test Cases

        #region EnumerateValues() Tests

        [Test]
        public void EnumerateValues_TEnumIsNotAnEnum_ThrowsException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                EnumEx.EnumerateValues<int>();
            });
        }

        [TestCaseSource(nameof(TEnumTestCaseData))]
        public void EnumerateValues_TEnumIsAnEnum_EnumeratesEnumValues(Type tEnum)
            => GetType()
                .GetMethod(
                    nameof(EnumerateValues_TEnumIsAnEnum_EnumeratesEnumValues),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(tEnum)
                .Invoke(null, Array.Empty<object>());

        private static void EnumerateValues_TEnumIsAnEnum_EnumeratesEnumValues<TEnum>()
            where TEnum : struct, IConvertible
        {
            var values = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>();

            var result = EnumEx.EnumerateValues<TEnum>()
                .ToArray();

            result.ShouldBeSequenceEqualTo(values);
        }

        #endregion EnumerateValues() Tests

        #region EnumerateValuesWithDescriptions() Tests

        [Test]
        public void EnumerateValuesWithDescriptions_TEnumIsNotAnEnum_ThrowsException()
        {
            Should.Throw<ArgumentException>(() =>
            {
                EnumEx.EnumerateValuesWithDescriptions<int>();
            });
        }

        [TestCaseSource(nameof(TEnumTestCaseData))]
        public void EnumerateValuesAndDescriptions_TEnumIsAnEnum_EnumeratesEnumValues(Type tEnum)
            => GetType()
                .GetMethod(
                    nameof(EnumerateValues_TEnumIsAnEnum_EnumeratesEnumValues),
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(tEnum)
                .Invoke(null, Array.Empty<object>());

        private static void EnumerateValuesAndDescriptions_TEnumIsAnEnum_EnumeratesEnumValues<TEnum>()
            where TEnum : struct, IConvertible
        {
            var valuesWithDescriptions = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(x => (
                    value: x,
                    description: typeof(TEnum).GetField(x.ToString())
                        .GetCustomAttribute<DescriptionAttribute>()
                        ?.Description));

            var result = EnumEx.EnumerateValuesWithDescriptions<TEnum>()
                .ToArray();

            result.ShouldBeSequenceEqualTo(valuesWithDescriptions);
        }

        #endregion EnumerateValuesWithDescriptions() Tests
    }
}
