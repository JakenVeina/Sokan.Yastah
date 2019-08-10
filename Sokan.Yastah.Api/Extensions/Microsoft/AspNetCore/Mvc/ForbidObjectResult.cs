using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.AspNetCore.Mvc
{
    [DefaultStatusCode(403)]
    public class ForbidObjectResult : ObjectResult
    {
        public ForbidObjectResult(object value)
            : base(value)
        {
            StatusCode = DefaultStatusCode;
        }

        private const int DefaultStatusCode = StatusCodes.Status403Forbidden;
    }
}
