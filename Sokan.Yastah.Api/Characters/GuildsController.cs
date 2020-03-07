using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Characters;

namespace Sokan.Yastah.Api.Characters
{
    public class GuildsController
        : CharactersControllerBase
    {
        #region Construction

        public GuildsController(
                    ICharacterGuildsOperations characterGuildsOperations,
                    ILogger<GuildsController> logger)
                : base(logger)
            => _characterGuildsOperations = characterGuildsOperations;

        #endregion Construction

        #region Actions

        [HttpPost(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> New(
                [FromBody]CharacterGuildCreationModel body)
            => TranslateOperation(await _characterGuildsOperations.CreateAsync(
                creationModel: body,
                cancellationToken: HttpContext.RequestAborted));

        [HttpDelete(DefaultAreaRouteTemplate)]
        public async Task<IActionResult> Delete(
                long id)
            => TranslateOperation(await _characterGuildsOperations.DeleteAsync(
                guildId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet(DefaultAreaActionRouteTemplate)]
        public async Task<IActionResult> Identities()
            => TranslateOperation(await _characterGuildsOperations.GetIdentitiesAsync(
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut(DefaultAreaRouteTemplate)]
        public async Task<IActionResult> Update(
                long id,
                [FromBody]CharacterGuildUpdateModel body)
            => TranslateOperation(await _characterGuildsOperations.UpdateAsync(
                guildId: id,
                updateModel: body,
                cancellationToken: HttpContext.RequestAborted));

        #endregion Actions

        #region State

        private readonly ICharacterGuildsOperations _characterGuildsOperations;

        #endregion State
    }
}
