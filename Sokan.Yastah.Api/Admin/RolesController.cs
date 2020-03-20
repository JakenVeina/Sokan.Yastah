using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Roles;

namespace Sokan.Yastah.Api.Admin
{
    public class RolesController
        : AdminControllerBase
    {
        #region Construction

        public RolesController(
                    ILogger<RolesController> logger,
                    IRolesOperations rolesOperations)
                : base(logger)
            => _rolesOperations = rolesOperations;

        #endregion Construction

        #region Actions

        [HttpPost(DefaultAreaActionRouteTemplate)]
        public Task<IActionResult> New(
                [FromBody]RoleCreationModel body)
            => PerformOperationAsync(() => _rolesOperations.CreateAsync(
                creationModel: body,
                cancellationToken: HttpContext.RequestAborted));

        [HttpDelete(DefaultAreaRouteTemplate)]
        public Task<IActionResult> Delete(
                long id)
            => PerformOperationAsync(() => _rolesOperations.DeleteAsync(
                roleId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaIdActionRouteTemplate)]
        public Task<IActionResult> Detail(
                long id)
            => PerformOperationAsync(() => _rolesOperations.GetDetailAsync(
                roleId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public ValueTask<IActionResult> Identities()
            => PerformOperationAsync(() => _rolesOperations.GetIdentitiesAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaRouteTemplate)]
        public Task<IActionResult> Update(
                long id,
                [FromBody]RoleUpdateModel body)
            => PerformOperationAsync(() => _rolesOperations.UpdateAsync(
                roleId: id,
                updateModel: body,
                cancellationToken: HttpContext.RequestAborted));

        #endregion Actions
        
        #region State
        
        private readonly IRolesOperations _rolesOperations;

        #endregion State
    }
}
