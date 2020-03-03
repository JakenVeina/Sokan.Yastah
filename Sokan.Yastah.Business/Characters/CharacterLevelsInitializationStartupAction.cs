using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;

using Sokan.Yastah.Business.Administration;
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
            if(!_yastahAutoMigrationStartupAction.WhenDone.IsCompletedSuccessfully)
                YastahDbContextLogMessages.ContextMigrationAwaiting(_logger);
            await _yastahAutoMigrationStartupAction.WhenDone;

            CharactersLogMessages.CharacterLevelDefinitionsInitializing(_logger);

            var administrationActionsRepository = serviceProvider.GetRequiredService<IAdministrationActionsRepository>();
            var characterLevelsRepository = serviceProvider.GetRequiredService<ICharacterLevelsRepository>();

            using var transactionScope = _transactionScopeFactory.CreateScope();
            TransactionsLogMessages.TransactionScopeCreated(_logger);

            var level1Exists = await characterLevelsRepository.AnyDefinitionsAsync(
                level:               1,
                experienceThreshold: 0,
                isDeleted:           false,
                cancellationToken:   cancellationToken);

            if (!level1Exists)
            {
                CharactersLogMessages.CharacterLevelsNotInitialized(_logger);
                
                var actionId = await administrationActionsRepository.CreateAsync(
                    (int)CharacterManagementAdministrationActionType.LevelDefinitionsInitialized,
                    _systemClock.UtcNow,
                    null,
                    cancellationToken);
                AdministrationLogMessages.AdministrationActionCreated(_logger, actionId);

                await characterLevelsRepository.MergeDefinitionAsync(
                    1,
                    0,
                    false,
                    actionId,
                    cancellationToken);
                CharactersLogMessages.CharacterLevelDefinition1Created(_logger);
            }

            TransactionsLogMessages.TransactionScopeCommitting(_logger);
            transactionScope.Complete();

            CharactersLogMessages.CharacterLevelDefinitionsInitialized(_logger);
        }

        private readonly ISystemClock _systemClock;
        private readonly ITransactionScopeFactory _transactionScopeFactory;
        private readonly IYastahAutoMigrationStartupAction _yastahAutoMigrationStartupAction;
    }
}
