using Microsoft.AspNetCore.Mvc;

using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;

namespace Sokan.Yastah.Api
{
    public abstract class ApiControllerBase
        : ControllerBaseEx
    {
        public const string DefaultAreaIdActionRouteTemplate    = "{prefix}/[area]/[controller]/{id?}/[action]";
        public const string DefaultAreaRouteTemplate            = "{prefix}/[area]/[controller]/{id?}";
        public const string DefaultAreaActionRouteTemplate      = "{prefix}/[area]/[controller]/[action]/{id?}";
        public const string DefaultIdActionRouteTemplate        = "{prefix}/[controller]/{id?}/[action]";
        public const string DefaultRouteTemplate                = "{prefix}/[controller]/{id?}";
        public const string DefaultActionRouteTemplate          = "{prefix}/[controller]/[action]/{id?}";

        internal protected IActionResult TranslateOperation(OperationResult result)
            => result.IsSuccess
                ? Ok()
                : TranslateOperationError(result.Error);

        internal protected IActionResult TranslateOperation<T>(OperationResult<T> result)
            => result.IsSuccess
                ? Ok(result.Value)
                : TranslateOperationError(result.Error);

        private IActionResult TranslateOperationError(IOperationError error)
            => error switch
            {
                UnauthenticatedUserError _      => Unauthorized(error),
                InsufficientPermissionsError _  => Forbid(error),
                DataNotFoundError _             => NotFound(error),
                _                               => BadRequest(error),
            };
    }
}
