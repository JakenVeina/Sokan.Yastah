using System.Threading.Tasks;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace Sokan.Yastah.Api.Antiforgery
{
    public class AntiforgeryMiddleware
    {
        public AntiforgeryMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IAntiforgery antiforgery,
            IOptions<AntiforgeryConfiguration> configuration)
        {
            var isRequestValid = await antiforgery.IsRequestValidAsync(context);
            if (!isRequestValid)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(AntiforgeryValidationError.Default));

                return;
            }

            var tokenSet = antiforgery.GetAndStoreTokens(context);

            context.Response.Cookies.Append(
                key: configuration.Value.RequestTokenCookieName ?? AntiforgeryDefaults.RequestTokenCookieName,
                value: tokenSet.RequestToken,
                options: new CookieOptions()
                {
                    SameSite = SameSiteMode.Strict,
                    Secure = true
                });

            await _next.Invoke(context);
        }

        private readonly RequestDelegate _next;
    }
}
