using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sokan.Yastah.Api.Characters
{
    [Area("characters")]
    public abstract class CharactersControllerBase
        : ApiControllerBase
    {
        #region Construction

        protected CharactersControllerBase(
                ILogger logger)
            : base(logger) { }

        #endregion Construction
    }
}
