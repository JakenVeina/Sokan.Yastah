using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Permissions;

namespace Sokan.Yastah.Api.Admin
{
    public sealed class PermissionsController
        : AdminControllerBase
    {
        #region Construction

        public PermissionsController(
                    ILogger<PermissionsController> logger,
                    IPermissionsOperations permissionsOperations)
                : base(logger)
            => _permissionsOperations = permissionsOperations;

        #endregion Construction

        #region Actions

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public ValueTask<IActionResult> Descriptions()
            => PerformOperationAsync(() => _permissionsOperations.GetDescriptionsAsync(
                HttpContext.RequestAborted));

        #endregion Actions

        #region State
        
        private readonly IPermissionsOperations _permissionsOperations;

        #endregion State
    }
}
