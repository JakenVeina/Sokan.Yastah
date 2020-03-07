using System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Api.Antiforgery
{
    internal static class AntiforgeryLogMessages
    {
        public static void RequestValidationFailed(
                ILogger logger)
            => _requestValidationFailed.Invoke(
                logger);
        private static readonly Action<ILogger> _requestValidationFailed
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(2001, nameof(RequestValidationFailed)),
                    "HTTP Request failed Antiforgery validation")
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
                    new EventId(4001, nameof(RequestValidating)),
                    "Performing HTTP Request Antiforgery validation:\r\n\tHttpContext: {HttpContext}")
                .WithoutException();

        public static void RequestValidationSucceeded(
                ILogger logger)
            => _requestValidationSucceeded.Invoke(
                logger);
        private static readonly Action<ILogger> _requestValidationSucceeded
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4002, nameof(RequestValidationSucceeded)),
                    "HTTP Request Antiforgery validation succeeded")
                .WithoutException();

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
                    new EventId(4003, nameof(RequestTokenCookieAttached)),
                    "HTTP Antiforgery Request Token attached:\r\n\tCookieKey: {CookieKey}\r\n\tRequestToken: {RequestToken}")
                .WithoutException();
    }
}
