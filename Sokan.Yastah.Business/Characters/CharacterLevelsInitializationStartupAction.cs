using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Data;
using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Business.Characters
{
    [ServiceBinding(ServiceLifetime.Singleton)]
    public class CharacterLevelsInitializationStartupAction
        : ScopedStartupActionBase
    {
        public CharacterLevelsInitializationStartupAction(
                ILogger<CharacterLevelsInitializationStartupAction> logger,
                IServiceScopeFactory serviceScopeFactory,
                ISystemClock systemClock,
                ITransactionScopeFactory transactionScopeFactory,
                IYastahAutoMigrationStartupAction yastahAutoMigrationStartupAction)
            : base(
                logger,
                serviceScopeFactory)
        {
            _systemClock = systemClock;
            _transactionScopeFactory = transactionScopeFactory;
            _yastahAutoMigrationStartupAction = yastahAutoMigrationStartupAction;
        }

        protected override async Task OnStartingAsync(
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            await _yastahAutoMigrationStartupAction.WhenDone;

            var administrationActionsRepository = serviceProvider.GetRequiredService<IAdministrationActionsRepository>();
            var characterLevelsRepository = serviceProvider.GetRequiredService<ICharacterLevelsRepository>();

            using var transactionScope = _transactionScopeFactory.CreateScope();

            var level1Exists = await characterLevelsRepository.AnyDefinitionsAsync(
                level:               1,
                experienceThreshold: 0,
                isDeleted:           false,
                cancellationToken:   cancellationToken);

            if (!level1Exists)
            {
                var actionId = await administrationActionsRepository.CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsInitialized,
                    _systemClock.UtcNow,
                    null,
                    cancellationToken);

                await characterLevelsRepository.MergeDefinitionAsync(
                    1,
                    0,
                    false,
                    actionId,
                    cancellationToken);
            }

            transactionScope.Complete();
        }

        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IYastahAutoMigrationStartupAction _yastahAutoMigrationStartupAction;
    }
}
