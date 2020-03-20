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
        public Task<IActionResult> New(
                long guildId,
                [FromBody]CharacterGuildDivisionCreationModel body)
            => PerformOperationAsync(() => _characterGuildDivisionsOperations.CreateAsync(
                guildId: guildId,
                creationModel: body,
                cancellationToken: HttpContext.RequestAborted));

        [HttpDelete("{prefix}/[area]/guilds/{guildId}/[controller]/{id}")]
        public Task<IActionResult> Delete(
                long guildId,
                long id)
            => PerformOperationAsync(() => _characterGuildDivisionsOperations.DeleteAsync(
                guildId: guildId,
                divisionId: id,
                cancellationToken: HttpContext.RequestAborted));

        [HttpGet("{prefix}/[area]/guilds/{guildId}/[controller]/[action]")]
        public Task<IActionResult> Identities(
                long guildId)
            => PerformOperationAsync(() => _characterGuildDivisionsOperations.GetIdentitiesAsync(
                guildId: guildId,
                cancellationToken: HttpContext.RequestAborted));

        [HttpPut("{prefix}/[area]/guilds/{guildId}/[controller]/{id}")]
        public Task<IActionResult> Update(
                long guildId,
                long id,
                [FromBody]CharacterGuildDivisionUpdateModel body)
            => PerformOperationAsync(() => _characterGuildDivisionsOperations.UpdateAsync(
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
