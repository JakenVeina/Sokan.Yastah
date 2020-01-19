using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Sokan.Yastah.Business.Characters;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Api.Characters
{
    public class LevelsController
        : CharactersControllerBase
    {
        public LevelsController(
                ICharacterLevelsOperations characterLevelsOperations)
            => _characterLevelsOperations = characterLevelsOperations;

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> Definitions()
            => TranslateOperation(await _characterLevelsOperations.GetDefinitionsAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> ExperienceDiffs(
                [FromBody]IReadOnlyList<int> body)
            => TranslateOperation(await _characterLevelsOperations.UpdateExperienceDiffsAsync(
                experienceDiffs: body,
                cancellationToken: HttpContext.RequestAborted));

        private readonly ICharacterLevelsOperations _characterLevelsOperations;
    }
}
