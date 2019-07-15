using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Net.Http.Headers;

namespace Sokan.Yastah.Api.Authentication
{
    [Route("authentication")]
    public class AuthenticationController
        : ApiControllerBase
    {
        [HttpGet("/api/authentication/challenge")]
        new public IActionResult Challenge()
        {
            var properties = new AuthenticationProperties();
            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.Referer, out var referer))
                properties.RedirectUri = referer;

            return Challenge(properties, ApiAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("/api/authentication/signout")]
        public IActionResult SignOut()
        {
            var properties = new AuthenticationProperties();
            if (HttpContext.Request.Headers.TryGetValue(HeaderNames.Referer, out var referer))
                properties.RedirectUri = referer;

            return SignOut(properties, ApiAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
