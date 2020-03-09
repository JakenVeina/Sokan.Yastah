using System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Api.Antiforgery
{
    internal static class AntiforgeryLogMessages
    {
        public enum EventType
        {
            RequestValidating           = ApiLogEventType.Antiforgery + 0x0001,
            RequestValidationFailed     = ApiLogEventType.Antiforgery + 0x0002,
            RequestValidationSucceeded  = ApiLogEventType.Antiforgery + 0x0003,
            RequestTokenCookieAttached  = ApiLogEventType.Antiforgery + 0x0004
        }

        public static void RequestTokenCookieAttached(
                ILogger logger,
                string cookieKey,
                string requestToken)
            => _requestTokenCookieAttached.Invoke(
                logger,
                cookieKey,
                requestToken);
        private static readonly Action<ILogger, string, string> _requestTokenCookieAttached
            = LoggerMessage.Define<string, string>(
                    LogLevel.Debug,
                    EventType.RequestTokenCookieAttached.ToEventId(),
                    "HTTP Antiforgery Request Token attached:\r\n\tCookieKey: {CookieKey}\r\n\tRequestToken: {RequestToken}")
                .WithoutException();

        public static void RequestValidating(
                ILogger logger,
                HttpContext httpContext)
            => _requestValidating.Invoke(
                logger,
                httpContext);
        private static readonly Action<ILogger, HttpContext> _requestValidating
            = LoggerMessage.Define<HttpContext>(
                    LogLevel.Debug,
                    EventType.RequestValidating.ToEventId(),
                    "Performing HTTP Request Antiforgery validation:\r\n\tHttpContext: {HttpContext}")
                .WithoutException();

        public static void RequestValidationFailed(
                ILogger logger)
            => _requestValidationFailed.Invoke(
                logger);
        private static readonly Action<ILogger> _requestValidationFailed
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    EventType.RequestValidationFailed.ToEventId(),
                    "HTTP Request failed Antiforgery validation")
                .WithoutException();

        public static void RequestValidationSucceeded(
                ILogger logger)
            => _requestValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _requestValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.RequestValidationSucceeded.ToEventId(),
                    "HTTP Request Antiforgery validation succeeded")
                .WithoutException();
    }
}
