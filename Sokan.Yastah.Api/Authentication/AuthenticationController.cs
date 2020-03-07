using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Sokan.Yastah.Api.Authentication
{
    public sealed class AuthenticationController
        : ApiControllerBase
    {
        #region Construction

        public AuthenticationController(
                ILogger<AuthenticationController> logger)
            : base(logger) { }

        #endregion Construction

        #region Actions

        [HttpGet(DefaultActionRouteTemplate)]
        new public IActionResult Challenge()
        {
            var properties = new AuthenticationProperties();
            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.Referer, out var referer))
                properties.RedirectUri = referer;

            return Challenge(properties, ApiAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet(DefaultActionRouteTemplate)]
        public IActionResult SignOut()
        {
            var properties = new AuthenticationProperties();
            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.Referer, out var referer))
                properties.RedirectUri = referer;

            return SignOut(properties, ApiAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion Actions
    }
}
