using System.Threading.Tasks;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryMiddleware
    {
        #region Construction

        public AntiforgeryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion Construction

        #region Middleware

        public async Task InvokeAsync(
            HttpContext context,
            IAntiforgery antiforgery,
            ILogger<AntiforgeryMiddleware> logger,
            IOptions<AntiforgeryConfiguration> configuration)
        {
            AntiforgeryLogMessages.RequestValidating(logger, context);

            var isRequestValid = await antiforgery.IsRequestValidAsync(context);
            if (!isRequestValid)
            {
                AntiforgeryLogMessages.RequestValidationFailed(logger);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(AntiforgeryValidationError.Default));

                return;
            }
            AntiforgeryLogMessages.RequestValidationSucceeded(logger);

            var tokenSet = antiforgery.GetAndStoreTokens(context);

            var requestTokenCookieKey = configuration.Value.RequestTokenCookieName ?? AntiforgeryDefaults.RequestTokenCookieName;
            context.Response.Cookies.Append(
                key: requestTokenCookieKey,
                value: tokenSet.RequestToken,
                options: new CookieOptions()
                {
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                });
            AntiforgeryLogMessages.RequestTokenCookieAttached(logger, requestTokenCookieKey, tokenSet.RequestToken);

            await _next.Invoke(context);
        }

        #endregion Middleware

        #region State
        
        private readonly RequestDelegate _next;

        #endregion State
    }
}
