using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Sokan.Yastah.Business.Characters;

namespace Sokan.Yastah.Api.Characters
{
    public class DivisionsController
        : CharactersControllerBase
    {
        public DivisionsController(
                ICharacterGuildDivisionsOperations characterGuildDivisionsOperations)
            => _characterGuildDivisionsOperations = characterGuildDivisionsOperations;

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

        private readonly ICharacterGuildDivisionsOperations _characterGuildDivisionsOperations;
    }
}
