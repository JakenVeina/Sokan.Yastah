using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Api.Characters
{
    public class LevelsController
        : CharactersControllerBase
    {
        #region Construction

        public LevelsController(
                    ICharacterLevelsOperations characterLevelsOperations,
                    ILogger<LevelsController> logger)
                : base(logger)
            => _characterLevelsOperations = characterLevelsOperations;

        #endregion Construction

        #region Actions

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public ValueTask<IActionResult> Definitions()
            => PerformOperationAsync(() => _characterLevelsOperations.GetDefinitionsAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaActionRouteTemplate)]
        public Task<IActionResult> ExperienceDiffs(
                [FromBody]IReadOnlyList<int> body)
            => PerformOperationAsync(() => _characterLevelsOperations.UpdateExperienceDiffsAsync(
                experienceDiffs: body,
                cancellationToken: HttpContext.RequestAborted));

        #endregion Actions

        #region State

        private readonly ICharacterLevelsOperations _characterLevelsOperations;

        #endregion State
    }
}
