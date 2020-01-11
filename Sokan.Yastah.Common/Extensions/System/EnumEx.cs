using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class EnumEx
    {
        public static IEnumerable<TEnum> EnumerateValues<TEnum>()
                where TEnum : struct, IConvertible
            => EnumerateEnumValueFields<TEnum>()
                .Select(x => (TEnum)x.GetValue(null)!);

        public static IEnumerable<(TEnum value, string description)> EnumerateValuesWithDescriptions<TEnum>()
                where TEnum : struct, IConvertible
            => EnumerateEnumValueFields<TEnum>()
                .Select(x =>
                {
                    var value = (TEnum)x.GetValue(null)!;
                    var description = x.GetCustomAttribute<DescriptionAttribute>()
                            ?.Description
                        ?? throw new ArgumentException($"Enum value {typeof(TEnum).Name}.{value.ToString()} does not have an attached {nameof(DescriptionAttribute)}", nameof(TEnum));

                    return (value, description);
                });

        private static IEnumerable<FieldInfo> EnumerateEnumValueFields<TEnum>()
                where TEnum : struct, IConvertible
        {
            var tEnum = typeof(TEnum);

            if (!tEnum.IsEnum)
                throw new ArgumentException("Must be an enum type", nameof(TEnum));

            return tEnum.GetMembers()
                .OfType<FieldInfo>()
                .Where(x => !x.IsSpecialName);
        }
    }
}
