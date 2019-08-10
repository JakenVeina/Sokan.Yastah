using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace Sokan.Yastah.Api.Authentication
{
    public class AuthenticationController
        : ApiControllerBase
    {
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
    }
}
