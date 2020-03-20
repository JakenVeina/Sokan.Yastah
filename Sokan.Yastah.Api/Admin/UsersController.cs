using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Users;

namespace Sokan.Yastah.Api.Admin
{
    public class UsersController
        : AdminControllerBase
    {
        #region Construction

        public UsersController(
                    ILogger<UsersController> logger,
                    IUsersOperations usersOperations)
                : base(logger)
            => _usersOperations = usersOperations;

        #endregion Construction

        #region Actions
        
        [HttpGet(DefaultAreaIdActionRouteTemplate)]
        public Task<IActionResult> Detail(
                ulong id)
            => PerformOperationAsync(() => _usersOperations.GetDetailAsync(
                userId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public Task<IActionResult> Overviews()
            => PerformOperationAsync(() => _usersOperations.GetOverviewsAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaRouteTemplate)]
        public Task<IActionResult> Update(
                ulong id,
                [FromBody]UserUpdateModel body)
            => PerformOperationAsync(() => _usersOperations.UpdateAsync(
                userId: id,
                updateModel: body,
                cancellationToken: HttpContext.RequestAborted));

        #endregion Actions

        #region State

        private readonly IUsersOperations _usersOperations;

        #endregion State
    }
}
