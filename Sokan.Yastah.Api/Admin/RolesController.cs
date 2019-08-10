using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Sokan.Yastah.Business.Roles;

namespace Sokan.Yastah.Api.Admin
{
    public class RolesController
        : AdminControllerBase
    {
        public RolesController(
                IRolesOperations rolesOperations)
            => _rolesOperations = rolesOperations;

        [HttpPost(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> New(
                [FromBody]RoleCreationModel body)
            => TranslateOperation(await _rolesOperations.CreateAsync(
                creationModel: body,
                cancellationToken: HttpContext.RequestAborted));

        [HttpDelete(DefaultAreaRouteTemplate)]
        public async Task<IActionResult> Delete(
                long id)
            => TranslateOperation(await _rolesOperations.DeleteAsync(
                roleId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaIdActionRouteTemplate)]
        public async Task<IActionResult> Detail(
                long id)
            => TranslateOperation(await _rolesOperations.GetDetailAsync(
                roleId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> Identities()
            => TranslateOperation(await _rolesOperations.GetIdentitiesAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaRouteTemplate)]
        public async Task<IActionResult> Update(
                long id,
                [FromBody]RoleUpdateModel body)
            => TranslateOperation(await _rolesOperations.UpdateAsync(
                roleId: id,
                updateModel: body,
                cancellationToken: HttpContext.RequestAborted));

        private readonly IRolesOperations _rolesOperations;
    }
}
