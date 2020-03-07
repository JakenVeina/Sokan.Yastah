using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Api.Admin
{
    [Area("admin")]
    public abstract class AdminControllerBase
        : ApiControllerBase
    {
        #region Construction

        protected AdminControllerBase(
                ILogger logger)
            : base(logger) { }

        #endregion Construction
    }
}
