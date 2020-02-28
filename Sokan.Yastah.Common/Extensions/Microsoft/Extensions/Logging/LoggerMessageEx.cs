using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerMessageEx
    {
        public static Action<ILogger, T1, T2, T3, T4, T5, T6, Exception?> Define<T1, T2, T3, T4, T5, T6>(
            LogLevel logLevel,
            EventId eventId,
            string formatString)
        {
            var formatter = CreateLogValuesFormatter(formatString, expectedNamedParameterCount: 6);

            return (logger, arg1, arg2, arg3, arg4, arg5, arg6, exception) =>
            {
                if (logger.IsEnabled(logLevel))
                    logger.Log(
                        logLevel,
                        eventId,
                        new LogValues<T1, T2, T3, T4, T5, T6>(formatter, arg1, arg2, arg3, arg4, arg5, arg6),
                        exception,
                        LogValues<T1, T2, T3, T4, T5, T6>.Callback);
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, Exception?> Define<T1, T2, T3, T4, T5, T6, T7>(
            LogLevel logLevel,
            EventId eventId,
            string formatString)
        {
            var formatter = CreateLogValuesFormatter(formatString, expectedNamedParameterCount: 7);

            return (logger, arg1, arg2, arg3, arg4, arg5, arg6, arg7, exception) =>
            {
                if (logger.IsEnabled(logLevel))
                    logger.Log(
                        logLevel,
                        eventId,
                        new LogValues<T1, T2, T3, T4, T5, T6, T7>(formatter, arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                        exception,
                        LogValues<T1, T2, T3, T4, T5, T6, T7>.Callback);
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, Exception?> Define<T1, T2, T3, T4, T5, T6, T7, T8>(
            LogLevel logLevel,
            EventId eventId,
            string formatString)
        {
            var formatter = CreateLogValuesFormatter(formatString, expectedNamedParameterCount: 8);

            return (logger, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, exception) =>
            {
                if (logger.IsEnabled(logLevel))
                    logger.Log(
                        logLevel,
                        eventId,
                        new LogValues<T1, T2, T3, T4, T5, T6, T7, T8>(formatter, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8),
                        exception,
                        LogValues<T1, T2, T3, T4, T5, T6, T7, T8>.Callback);
            };
        }

        private static LogValuesFormatter CreateLogValuesFormatter(string formatString, int expectedNamedParameterCount)
        {
            var logValuesFormatter = new LogValuesFormatter(formatString);

            var actualCount = logValuesFormatter.ValueNames.Count;
            if (actualCount != expectedNamedParameterCount)
            {
                throw new ArgumentException(
                    $"The format string '{formatString}' does not have the expected number of named parameters.Expected {expectedNamedParameterCount} parameter(s) but found {actualCount} parameter(s).");
            }

            return logValuesFormatter;
        }

        private readonly struct LogValues<T0, T1, T2, T3, T4, T5> : IReadOnlyList<KeyValuePair<string, object?>>
        {
            public static readonly Func<LogValues<T0, T1, T2, T3, T4, T5>, Exception?, string> Callback = (state, exception) => state.ToString();

            private readonly LogValuesFormatter _formatter;
            private readonly T0 _value0;
            private readonly T1 _value1;
            private readonly T2 _value2;
            private readonly T3 _value3;
            private readonly T4 _value4;
            private readonly T5 _value5;

            public int Count => 7;

            public KeyValuePair<string, object?> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[0], _value0);
                        case 1:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[1], _value1);
                        case 2:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[2], _value2);
                        case 3:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[3], _value3);
                        case 4:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[4], _value4);
                        case 5:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[5], _value5);
                        case 6:
                            return new KeyValuePair<string, object?>("{OriginalFormat}", _formatter.OriginalFormat);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
            {
                _formatter = formatter;
                _value0 = value0;
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
                _value5 = value5;
            }

            private object?[] ToArray() => new object?[] { _value0, _value1, _value2, _value3, _value4, _value5 };

            public override string ToString() => _formatter.Format(ToArray());

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly struct LogValues<T0, T1, T2, T3, T4, T5, T6> : IReadOnlyList<KeyValuePair<string, object?>>
        {
            public static readonly Func<LogValues<T0, T1, T2, T3, T4, T5, T6>, Exception?, string> Callback = (state, exception) => state.ToString();

            private readonly LogValuesFormatter _formatter;
            private readonly T0 _value0;
            private readonly T1 _value1;
            private readonly T2 _value2;
            private readonly T3 _value3;
            private readonly T4 _value4;
            private readonly T5 _value5;
            private readonly T6 _value6;

            public int Count => 8;

            public KeyValuePair<string, object?> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[0], _value0);
                        case 1:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[1], _value1);
                        case 2:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[2], _value2);
                        case 3:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[3], _value3);
                        case 4:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[4], _value4);
                        case 5:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[5], _value5);
                        case 6:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[6], _value6);
                        case 7:
                            return new KeyValuePair<string, object?>("{OriginalFormat}", _formatter.OriginalFormat);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
            {
                _formatter = formatter;
                _value0 = value0;
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
                _value5 = value5;
                _value6 = value6;
            }

            private object?[] ToArray() => new object?[] { _value0, _value1, _value2, _value3, _value4, _value5, _value6 };

            public override string ToString() => _formatter.Format(ToArray());

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private readonly struct LogValues<T0, T1, T2, T3, T4, T5, T6, T7> : IReadOnlyList<KeyValuePair<string, object?>>
        {
            public static readonly Func<LogValues<T0, T1, T2, T3, T4, T5, T6, T7>, Exception?, string> Callback = (state, exception) => state.ToString();

            private readonly LogValuesFormatter _formatter;
            private readonly T0 _value0;
            private readonly T1 _value1;
            private readonly T2 _value2;
            private readonly T3 _value3;
            private readonly T4 _value4;
            private readonly T5 _value5;
            private readonly T6 _value6;
            private readonly T7 _value7;

            public int Count => 9;

            public KeyValuePair<string, object?> this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[0], _value0);
                        case 1:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[1], _value1);
                        case 2:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[2], _value2);
                        case 3:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[3], _value3);
                        case 4:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[4], _value4);
                        case 5:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[5], _value5);
                        case 6:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[6], _value6);
                        case 7:
                            return new KeyValuePair<string, object?>(_formatter.ValueNames[7], _value6);
                        case 8:
                            return new KeyValuePair<string, object?>("{OriginalFormat}", _formatter.OriginalFormat);
                        default:
                            throw new IndexOutOfRangeException(nameof(index));
                    }
                }
            }

            public LogValues(LogValuesFormatter formatter, T0 value0, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
            {
                _formatter = formatter;
                _value0 = value0;
                _value1 = value1;
                _value2 = value2;
                _value3 = value3;
                _value4 = value4;
                _value5 = value5;
                _value6 = value6;
                _value7 = value7;
            }

            private object?[] ToArray() => new object?[] { _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7 };

            public override string ToString() => _formatter.Format(ToArray());

            public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            {
                for (int i = 0; i < Count; ++i)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}