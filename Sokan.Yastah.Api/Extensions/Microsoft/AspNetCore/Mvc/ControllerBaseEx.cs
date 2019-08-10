namespace Microsoft.AspNetCore.Mvc
{
    public abstract class ControllerBaseEx
        : ControllerBase
    {
        [NonAction]
        public virtual ForbidObjectResult Forbid(object value)
            => new ForbidObjectResult(value);
    }
}
