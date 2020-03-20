using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static Action<ILogger> WithoutException(
                this Action<ILogger, Exception?> action)
            => logger => action.Invoke(logger, null);

        public static Action<ILogger, T> WithoutException<T>(
                this Action<ILogger, T, Exception?> action)
            => (logger, value) => action.Invoke(logger, value, null);

        public static Action<ILogger, T1, T2> WithoutException<T1, T2>(
                this Action<ILogger, T1, T2, Exception?> action)
            => (logger, value1, value2) => action.Invoke(logger, value1, value2, null);

        public static Action<ILogger, T1, T2, T3> WithoutException<T1, T2, T3>(
                this Action<ILogger, T1, T2, T3, Exception?> action)
            => (logger, value1, value2, value3) => action.Invoke(logger, value1, value2, value3, null);

        public static Action<ILogger, T1, T2, T3, T4> WithoutException<T1, T2, T3, T4>(
                this Action<ILogger, T1, T2, T3, T4, Exception?> action)
            => (logger, value1, value2, value3, value4) => action.Invoke(logger, value1, value2, value3, value4, null);

        public static Action<ILogger, T1, T2, T3, T4, T5> WithoutException<T1, T2, T3, T4, T5>(
                this Action<ILogger, T1, T2, T3, T4, T5, Exception?> action)
            => (logger, value1, value2, value3, value4, value5) => action.Invoke(logger, value1, value2, value3, value4, value5, null);

        public static Action<ILogger, T1, T2, T3, T4, T5, T6> WithoutException<T1, T2, T3, T4, T5, T6>(
                this Action<ILogger, T1, T2, T3, T4, T5, T6, Exception?> action)
            => (logger, value1, value2, value3, value4, value5, value6) => action.Invoke(logger, value1, value2, value3, value4, value5, value6, null);

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7> WithoutException<T1, T2, T3, T4, T5, T6, T7>(
                this Action<ILogger, T1, T2, T3, T4, T5, T6, T7, Exception?> action)
            => (logger, value1, value2, value3, value4, value5, value6, value7) => action.Invoke(logger, value1, value2, value3, value4, value5, value6, value7, null);

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8> WithoutException<T1, T2, T3, T4, T5, T6, T7, T8>(
                this Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, Exception?> action)
            => (logger, value1, value2, value3, value4, value5, value6, value7, value8) => action.Invoke(logger, value1, value2, value3, value4, value5, value6, value7, value8, null);
    }
}
