using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    public class CharacterLevelsInitializationBehavior
        : IStartupHandler
    {
        public CharacterLevelsInitializationBehavior(
            IAdministrationActionsRepository administrationActionsRepository,
            ICharacterLevelsRepository characterLevelsRepository,
            ISystemClock systemClock,
            ITransactionScopeFactory transactionScopeFactory)
        {
            _administrationActionsRepository = administrationActionsRepository;
            _characterLevelsRepository = characterLevelsRepository;
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
        }

        public async Task OnStartupAsync(CancellationToken cancellationToken)
        {
            using var transactionScope = _transactionScopeFactory.CreateScope();

            var level1Exists = await _characterLevelsRepository.AnyAsync(
                level:               1,
                experienceThreshold: 0,
                isDeleted:           false,
                cancellationToken:   cancellationToken);

            if (!level1Exists)
            {
                var actionId = await _administrationActionsRepository.CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsInitialized,
                    _systemClock.UtcNow,
                    null,
                    cancellationToken);

                await _characterLevelsRepository.MergeDefinitionAsync(
                    1,
                    0,
                    false,
                    actionId,
                    cancellationToken);
            }

            transactionScope.Complete();
        }

        private readonly IAdministrationActionsRepository _administrationActionsRepository;
        private readonly ICharacterLevelsRepository _characterLevelsRepository;
        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;

        [OnConfigureServices]
        public static void OnConfigureServices(IServiceCollection services)
            => services
                .AddTransient<IStartupHandler, CharacterLevelsInitializationBehavior>();
    }
}
