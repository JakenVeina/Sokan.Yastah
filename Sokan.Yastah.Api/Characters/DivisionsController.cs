using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Characters;

namespace Sokan.Yastah.Api.Characters
{
    public class DivisionsController
        : CharactersControllerBase
    {
        #region Construction

        public DivisionsController(
                    ICharacterGuildDivisionsOperations characterGuildDivisionsOperations,
                    ILogger<DivisionsController> logger)
                : base(logger)
            => _characterGuildDivisionsOperations = characterGuildDivisionsOperations;

        #endregion Construction

        #region Actions

        [HttpPost("{prefix}/[area]/guilds/{guildId}/[controller]/[action]")]
        public async Task<IActionResult> New(
                long guildId,
                [FromBody]CharacterGuildDivisionCreationModel body)
            => TranslateOperation(await _characterGuildDivisionsOperations.CreateAsync(
                guildId: guildId,
                creationModel: body,
                cancellationToken: HttpContext.RequestAborted));

        [HttpDelete("{prefix}/[area]/guilds/{guildId}/[controller]/{id}")]
        public async Task<IActionResult> Delete(
                long guildId,
                long id)
            => TranslateOperation(await _characterGuildDivisionsOperations.DeleteAsync(
                guildId: guildId,
                divisionId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet("{prefix}/[area]/guilds/{guildId}/[controller]/[action]")]
        public async Task<IActionResult> Identities(
                long guildId)
            => TranslateOperation(await _characterGuildDivisionsOperations.GetIdentitiesAsync(
                guildId: guildId,
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut("{prefix}/[area]/guilds/{guildId}/[controller]/{id}")]
        public async Task<IActionResult> Update(
                long guildId,
                long id,
                [FromBody]CharacterGuildDivisionUpdateModel body)
            => TranslateOperation(await _characterGuildDivisionsOperations.UpdateAsync(
                guildId: guildId,
                divisionId: id,
                updateModel: body,
                cancellationToken: HttpContext.RequestAborted));

        #endregion Actions

        #region State

        private readonly ICharacterGuildDivisionsOperations _characterGuildDivisionsOperations;

        #endregion State
    }
}
