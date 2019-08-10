using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Sokan.Yastah.Business.Permissions;

namespace Sokan.Yastah.Api.Admin
{
    public class PermissionsController
        : AdminControllerBase
    {
        public PermissionsController(
                IPermissionsOperations permissionsOperations)
            => _permissionsOperations = permissionsOperations;

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> Descriptions()
            => TranslateOperation(await _permissionsOperations.GetDescriptionsAsync(
                HttpContext.RequestAborted));

        private readonly IPermissionsOperations _permissionsOperations;
    }
}
