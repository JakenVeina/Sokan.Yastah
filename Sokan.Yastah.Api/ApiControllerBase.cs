using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business;
using Sokan.Yastah.Business.Authorization;
using Sokan.Yastah.Common.OperationModel;
using Sokan.Yastah.Data;

namespace Sokan.Yastah.Api
{
    public abstract class ApiControllerBase
        : ControllerBaseEx
    {
        #region Constants

        public const string DefaultAreaIdActionRouteTemplate    = "{prefix}/[area]/[controller]/{id?}/[action]";
        public const string DefaultAreaRouteTemplate            = "{prefix}/[area]/[controller]/{id?}";
        public const string DefaultAreaActionRouteTemplate      = "{prefix}/[area]/[controller]/[action]/{id?}";
        public const string DefaultIdActionRouteTemplate        = "{prefix}/[controller]/{id?}/[action]";
        public const string DefaultRouteTemplate                = "{prefix}/[controller]/{id?}";
        public const string DefaultActionRouteTemplate          = "{prefix}/[controller]/[action]/{id?}";

        #endregion Constants

        #region Construction
        
        protected ApiControllerBase(
                ILogger logger)
            => _logger = logger;

        #endregion Construction

        #region Protected Members

        internal protected ILogger Logger
            => _logger;

        internal protected IActionResult TranslateOperation(
            OperationResult result)
        {
            ApiControllerLogMessages.OperationResultTranslating(_logger, result);
            var actionResult = result.IsSuccess
                ? Ok()
                : TranslateOperationError(result.Error);
            ApiControllerLogMessages.OperationResultTranslated(_logger, actionResult);

            return actionResult;
        }

        internal protected IActionResult TranslateOperation<T>(
            OperationResult<T> result)
        {
            ApiControllerLogMessages.OperationResultTranslating(_logger, result);
            var actionResult = result.IsSuccess
                ? Ok(result.Value)
                : TranslateOperationError(result.Error);
            ApiControllerLogMessages.OperationResultTranslated(_logger, actionResult);

            return actionResult;
        }

        #endregion Protected Members

        #region Private Members

        private IActionResult TranslateOperationError(OperationError error)
            => error switch
            {
                UnauthenticatedUserError _      => Unauthorized(error),
                InsufficientPermissionsError _  => Forbid(error),
                DataNotFoundError _             => NotFound(error),
                _                               => BadRequest(error),
            };

        #endregion Private Members

        #region State

        private readonly ILogger _logger;

        #endregion State
    }
}
