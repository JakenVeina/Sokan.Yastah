using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class EnumEx
    {
        public static IEnumerable<TEnum> EnumerateValues<TEnum>()
        {
            var tEnum = typeof(TEnum);

            if (!tEnum.IsEnum)
                throw new ArgumentException("Must be an enum type", nameof(TEnum));

            return tEnum.GetMembers()
                .OfType<FieldInfo>()
                .Where(x => !x.IsSpecialName)
                .Select(x => (TEnum)x.GetValue(null));
        }

        public static IEnumerable<(TEnum value, string description)> EnumerateValuesWithDescriptions<TEnum>()
        {
            var tEnum = typeof(TEnum);

            if (!tEnum.IsEnum)
                throw new ArgumentException("Must be an enum type", nameof(TEnum));

            return tEnum.GetMembers()
                .OfType<FieldInfo>()
                .Where(x => !x.IsSpecialName)
                .Select(x => (
                    value: (TEnum)x.GetValue(null),
                    description: x.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description));
        }
    }
}
