using System;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Characters
{
    internal static class CharactersLogMessages
    {
        public enum EventType
        {
            CharacterGuildCreating                          = BusinessLogEventType.Characters + 0x0001,
            CharacterGuildCreated                           = BusinessLogEventType.Characters + 0x0002,
            CharacterGuildDeleting                          = BusinessLogEventType.Characters + 0x0003,
            CharacterGuildDeleteFailed                      = BusinessLogEventType.Characters + 0x0004,
            CharacterGuildDeleted                           = BusinessLogEventType.Characters + 0x0005,
            CharacterGuildIdentitiesFetchingCurrent         = BusinessLogEventType.Characters + 0x0006,
            CharacterGuildIdentitiesFetchedCurrent          = BusinessLogEventType.Characters + 0x0007,
            CharacterGuildUpdating                          = BusinessLogEventType.Characters + 0x0008,
            CharacterGuildUpdateFailed                      = BusinessLogEventType.Characters + 0x0009,
            CharacterGuildUpdated                           = BusinessLogEventType.Characters + 0x000A,
            CharacterGuildNameValidationFailed              = BusinessLogEventType.Characters + 0x000B,
            CharacterGuildNameValidationSucceeded           = BusinessLogEventType.Characters + 0x000C,
            CharacterGuildDivisionCreating                  = BusinessLogEventType.Characters + 0x000D,
            CharacterGuildDivisionCreated                   = BusinessLogEventType.Characters + 0x000E,
            CharacterGuildIdValidationFailed                = BusinessLogEventType.Characters + 0x000F,
            CharacterGuildIdValidationSucceeded             = BusinessLogEventType.Characters + 0x0010,
            CharacterGuildDivisionDeleting                  = BusinessLogEventType.Characters + 0x0011,
            CharacterGuildDivisionDeleteFailed              = BusinessLogEventType.Characters + 0x0012,
            CharacterGuildDivisionDeleted                   = BusinessLogEventType.Characters + 0x0013,
            CharacterGuildDivisionIdentitiesFetchingCurrent = BusinessLogEventType.Characters + 0x0014,
            CharacterGuildDivisionIdentitiesFetchedCurrent  = BusinessLogEventType.Characters + 0x0015,
            CharacterGuildDivisionUpdating                  = BusinessLogEventType.Characters + 0x0016,
            CharacterGuildDivisionUpdateFailed              = BusinessLogEventType.Characters + 0x0017,
            CharacterGuildDivisionUpdated                   = BusinessLogEventType.Characters + 0x0018,
            CharacterGuildDivisionNameValidationFailed      = BusinessLogEventType.Characters + 0x0019,
            CharacterGuildDivisionNameValidationSucceeded   = BusinessLogEventType.Characters + 0x001A,
            CharacterLevelDefinitionsInitializing           = BusinessLogEventType.Characters + 0x001B,
            CharacterLevelDefinitionsInitialized            = BusinessLogEventType.Characters + 0x001C,
            CharacterLevelsNotInitialized                   = BusinessLogEventType.Characters + 0x001D,
            CharacterLevelDefinition1Created                = BusinessLogEventType.Characters + 0x001E,
            CharacterLevelDefinitionsFetchingCurrent        = BusinessLogEventType.Characters + 0x001F,
            CharacterLevelDefinitionsFetchedCurrent         = BusinessLogEventType.Characters + 0x0020,
            CharacterLevelDefinitionsUpdating               = BusinessLogEventType.Characters + 0x0021,
            CharacterLevelDefinitionsUpdated                = BusinessLogEventType.Characters + 0x0022,
            CharacterLevelDefinitionProposed                = BusinessLogEventType.Characters + 0x0023,
            CharacterLevelDefinitionValidationFailed        = BusinessLogEventType.Characters + 0x0024,
            CharacterLevelDefinitionDeleting                = BusinessLogEventType.Characters + 0x0025,
            CharacterLevelDefinitionDeleted                 = BusinessLogEventType.Characters + 0x0026,
            CharacterLevelDefinitionUpdating                = BusinessLogEventType.Characters + 0x0027,
            CharacterLevelDefinitionUpdated                 = BusinessLogEventType.Characters + 0x0028,
            CharacterLevelDefinitionsNoChangesGiven         = BusinessLogEventType.Characters + 0x0029,
            CharacterLevelDefinitionsCacheCleared           = BusinessLogEventType.Characters + 0x002A,
        }

        public static void CharacterGuildCreated(
                ILogger logger,
                long guildId)
            => _characterGuildCreated.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildCreated.ToEventId(),
                    "CharacterGuild deleted:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterGuildCreating(
                ILogger logger,
                CharacterGuildCreationModel creationModel,
                ulong performedById)
            => _characterGuildCreating.Invoke(
                logger,
                creationModel,
                performedById);
        private static readonly Action<ILogger, CharacterGuildCreationModel, ulong> _characterGuildCreating
            = LoggerMessage.Define<CharacterGuildCreationModel, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildCreating.ToEventId(),
                    "Creating CharacterGuild:\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildDeleted(
                ILogger logger,
                long guildId)
            => _characterGuildDeleted.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildDeleted
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDeleted.ToEventId(),
                    "CharacterGuild deleted:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterGuildDeleteFailed(
                ILogger logger,
                long guildId,
                OperationResult deleteResult)
            => _characterGuildDeleteFailed.Invoke(
                logger,
                guildId,
                deleteResult);
        private static readonly Action<ILogger, long, OperationResult> _characterGuildDeleteFailed
            = LoggerMessage.Define<long, OperationResult>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDeleteFailed.ToEventId(),
                    "CharacterGuild update failed:\r\n\tGuildId: {GuildId}\r\n\tDeleteResult: {DeleteResult}")
                .WithoutException();

        public static void CharacterGuildDeleting(
                ILogger logger,
                long guildId,
                ulong performedById)
            => _characterGuildDeleting.Invoke(
                logger,
                guildId,
                performedById);
        private static readonly Action<ILogger, long, ulong> _characterGuildDeleting
            = LoggerMessage.Define<long, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDeleting.ToEventId(),
                    "Deleting CharacterGuild:\r\n\tGuildId: {GuildId}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildDivisionCreated(
                ILogger logger,
                long guildId,
                long divisionId)
            => _characterGuildDivisionCreated.Invoke(
                logger,
                guildId,
                divisionId);
        private static readonly Action<ILogger, long, long> _characterGuildDivisionCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionCreated.ToEventId(),
                    "CharacterGuildDivision deleted:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
                .WithoutException();

        public static void CharacterGuildDivisionCreating(
                ILogger logger,
                long guildId,
                CharacterGuildDivisionCreationModel creationModel,
                ulong performedById)
            => _characterGuildDivisionCreating.Invoke(
                logger,
                guildId,
                creationModel,
                performedById);
        private static readonly Action<ILogger, long, CharacterGuildDivisionCreationModel, ulong> _characterGuildDivisionCreating
            = LoggerMessage.Define<long, CharacterGuildDivisionCreationModel, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionCreating.ToEventId(),
                    "Creating CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildDivisionDeleted(
                ILogger logger,
                long guildId,
                long divisionId)
            => _characterGuildDivisionDeleted.Invoke(
                logger,
                guildId,
                divisionId);
        private static readonly Action<ILogger, long, long> _characterGuildDivisionDeleted
            = LoggerMessage.Define<long, long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionDeleted.ToEventId(),
                    "CharacterGuildDivision deleted:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
                .WithoutException();

        public static void CharacterGuildDivisionDeleteFailed(
                ILogger logger,
                long guildId,
                long divisionId,
                OperationResult deleteResult)
            => _characterGuildDivisionDeleteFailed.Invoke(
                logger,
                guildId,
                divisionId,
                deleteResult);
        private static readonly Action<ILogger, long, long, OperationResult> _characterGuildDivisionDeleteFailed
            = LoggerMessage.Define<long, long, OperationResult>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionDeleteFailed.ToEventId(),
                    "CharacterGuildDivision update failed:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tDeleteResult: {DeleteResult}")
                .WithoutException();

        public static void CharacterGuildDivisionDeleting(
                ILogger logger,
                long guildId,
                long divisionId,
                ulong performedById)
            => _characterGuildDivisionDeleting.Invoke(
                logger,
                guildId,
                divisionId,
                performedById);
        private static readonly Action<ILogger, long, long, ulong> _characterGuildDivisionDeleting
            = LoggerMessage.Define<long, long, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionDeleting.ToEventId(),
                    "Deleting CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildDivisionIdentitiesFetchedCurrent(
                ILogger logger,
                long guildId)
            => _characterGuildDivisionIdentitiesFetchedCurrent.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildDivisionIdentitiesFetchedCurrent
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionIdentitiesFetchedCurrent.ToEventId(),
                    "Current CharacterGuildDivision identities fetched:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterGuildDivisionIdentitiesFetchingCurrent(
                ILogger logger,
                long guildId)
            => _characterGuildDivisionIdentitiesFetchingCurrent.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildDivisionIdentitiesFetchingCurrent
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionIdentitiesFetchingCurrent.ToEventId(),
                    "Fetching current CharacterGuildDivision identities:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterGuildDivisionNameValidationFailed(
                ILogger logger,
                string divisionName,
                OperationResult validationResult)
            => _characterGuildDivisionNameValidationFailed.Invoke(
                logger,
                divisionName,
                validationResult);
        private static readonly Action<ILogger, string, OperationResult> _characterGuildDivisionNameValidationFailed
            = LoggerMessage.Define<string, OperationResult>(
                    LogLevel.Warning,
                    EventType.CharacterGuildDivisionNameValidationFailed.ToEventId(),
                    "CharacterGuildDivision Name is not valid: {DivisionName}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void CharacterGuildDivisionNameValidationSucceeded(
                ILogger logger,
                string divisionName)
            => _characterGuildDivisionNameValidationSucceeded.Invoke(
                logger,
                divisionName);
        private static readonly Action<ILogger, string> _characterGuildDivisionNameValidationSucceeded
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionNameValidationSucceeded.ToEventId(),
                    "CharacterGuildDivision Name is valid: {DivisionName}")
                .WithoutException();

        public static void CharacterGuildDivisionUpdated(
                ILogger logger,
                long guildId,
                long divisionId)
            => _characterGuildDivisionUpdated.Invoke(
                logger,
                guildId,
                divisionId);
        private static readonly Action<ILogger, long, long> _characterGuildDivisionUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionUpdated.ToEventId(),
                    "CharacterGuildDivision updated:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
                .WithoutException();

        public static void CharacterGuildDivisionUpdating(
                ILogger logger,
                long guildId,
                long divisionId,
                CharacterGuildDivisionUpdateModel updateModel,
                ulong performedById)
            => _characterGuildDivisionUpdating.Invoke(
                logger,
                guildId,
                divisionId,
                updateModel,
                performedById);
        private static readonly Action<ILogger, long, long, CharacterGuildDivisionUpdateModel, ulong> _characterGuildDivisionUpdating
            = LoggerMessage.Define<long, long, CharacterGuildDivisionUpdateModel, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionUpdating.ToEventId(),
                    "Updating CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildDivisionUpdateFailed(
                ILogger logger,
                long guildId,
                long divisionId,
                OperationResult updateResult)
            => _characterGuildDivisionUpdateFailed.Invoke(
                logger,
                guildId,
                divisionId,
                updateResult);
        private static readonly Action<ILogger, long, long, OperationResult> _characterGuildDivisionUpdateFailed
            = LoggerMessage.Define<long, long, OperationResult>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionUpdateFailed.ToEventId(),
                    "CharacterGuildDivision update failed:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tUpdateResult: {UpdateResult}")
                .WithoutException();

        public static void CharacterGuildIdentitiesFetchedCurrent(
                ILogger logger)
            => _characterGuildIdentitiesFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterGuildIdentitiesFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterGuildIdentitiesFetchedCurrent.ToEventId(),
                    "Current CharacterGuildDivision identities fetched")
                .WithoutException();

        public static void CharacterGuildIdentitiesFetchingCurrent(
                ILogger logger)
            => _characterGuildIdentitiesFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterGuildIdentitiesFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterGuildIdentitiesFetchingCurrent.ToEventId(),
                    "Fetching current CharacterGuild identities")
                .WithoutException();

        public static void CharacterGuildIdValidationFailed(
                ILogger logger,
                long guildId,
                OperationResult validationResult)
            => _characterGuildIdValidationFailed.Invoke(
                logger,
                guildId,
                validationResult);
        private static readonly Action<ILogger, long, OperationResult> _characterGuildIdValidationFailed
            = LoggerMessage.Define<long, OperationResult>(
                    LogLevel.Warning,
                    EventType.CharacterGuildIdValidationFailed.ToEventId(),
                    "CharacterGuild Id is not valid: {GuildId}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void CharacterGuildIdValidationSucceeded(
                ILogger logger,
                long guildId)
            => _characterGuildIdValidationSucceeded.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildIdValidationSucceeded
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildIdValidationSucceeded.ToEventId(),
                    "CharacterGuild Id is valid: {GuildId}")
                .WithoutException();

        public static void CharacterGuildNameValidationFailed(
                ILogger logger,
                string guildName,
                OperationResult validationResult)
            => _characterGuildNameValidationFailed.Invoke(
                logger,
                guildName,
                validationResult);
        private static readonly Action<ILogger, string, OperationResult> _characterGuildNameValidationFailed
            = LoggerMessage.Define<string, OperationResult>(
                    LogLevel.Warning,
                    EventType.CharacterGuildNameValidationFailed.ToEventId(),
                    "CharacterGuild Name is not valid: {GuildName}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void CharacterGuildNameValidationSucceeded(
                ILogger logger,
                string guildName)
            => _characterGuildNameValidationSucceeded.Invoke(
                logger,
                guildName);
        private static readonly Action<ILogger, string> _characterGuildNameValidationSucceeded
            = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    EventType.CharacterGuildNameValidationSucceeded.ToEventId(),
                    "CharacterGuild Name is valid: {GuildName}")
                .WithoutException();

        public static void CharacterGuildUpdated(
                ILogger logger,
                long guildId)
            => _characterGuildUpdated.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildUpdated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    EventType.CharacterGuildUpdated.ToEventId(),
                    "CharacterGuild updated:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterGuildUpdateFailed(
                ILogger logger,
                long guildId,
                OperationResult updateResult)
            => _characterGuildUpdateFailed.Invoke(
                logger,
                guildId,
                updateResult);
        private static readonly Action<ILogger, long, OperationResult> _characterGuildUpdateFailed
            = LoggerMessage.Define<long, OperationResult>(
                    LogLevel.Debug,
                    EventType.CharacterGuildUpdateFailed.ToEventId(),
                    "CharacterGuild update failed:\r\n\tGuildId: {GuildId}\r\n\tUpdateResult: {UpdateResult}")
                .WithoutException();

        public static void CharacterGuildUpdating(
                ILogger logger,
                long guildId,
                CharacterGuildUpdateModel updateModel,
                ulong performedById)
            => _characterGuildUpdating.Invoke(
                logger,
                guildId,
                updateModel,
                performedById);
        private static readonly Action<ILogger, long, CharacterGuildUpdateModel, ulong> _characterGuildUpdating
            = LoggerMessage.Define<long, CharacterGuildUpdateModel, ulong>(
                    LogLevel.Debug,
                    EventType.CharacterGuildUpdating.ToEventId(),
                    "Updating CharacterGuild:\r\n\tGuildId: {GuildId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterLevelDefinition1Created(
                ILogger logger)
            => _characterLevelDefinition1Created.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinition1Created
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinition1Created.ToEventId(),
                    "CharacterLevelDefinition created for Level 1")
                .WithoutException();

        public static void CharacterLevelDefinitionDeleted(
                ILogger logger,
                int level)
            => _characterLevelDefinitionDeleted.Invoke(
                logger,
                level);
        private static readonly Action<ILogger, int> _characterLevelDefinitionDeleted
            = LoggerMessage.Define<int>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionDeleted.ToEventId(),
                    "CharacterLevelDefinitions deleted:\r\n\tLevel: {Level}")
                .WithoutException();

        public static void CharacterLevelDefinitionDeleting(
                ILogger logger,
                int level)
            => _characterLevelDefinitionDeleting.Invoke(
                logger,
                level);
        private static readonly Action<ILogger, int> _characterLevelDefinitionDeleting
            = LoggerMessage.Define<int>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionDeleting.ToEventId(),
                    "Deleting CharacterLevelDefinition:\r\n\tLevel: {Level}")
                .WithoutException();

        public static void CharacterLevelDefinitionProposed(
                ILogger logger,
                int level,
                int experienceThreshold,
                int previousExperienceThreshold)
            => _characterLevelDefinitionProposed.Invoke(
                logger,
                level,
                experienceThreshold,
                previousExperienceThreshold);
        private static readonly Action<ILogger, int, int, int> _characterLevelDefinitionProposed
            = LoggerMessage.Define<int, int, int>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionProposed.ToEventId(),
                    "CharacterLevelDefinition proposed:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}\r\n\tPreviousExperienceThreshold: {PreviousExperienceThreshold}")
                .WithoutException();

        public static void CharacterLevelDefinitionsCacheCleared(
                ILogger logger)
            => _characterLevelDefinitionsCacheCleared.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsCacheCleared
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsCacheCleared.ToEventId(),
                    "CharacterLevelDefinitions cache cleared")
                .WithoutException();

        public static void CharacterLevelDefinitionsFetchedCurrent(
                ILogger logger)
            => _characterLevelDefinitionsFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsFetchedCurrent.ToEventId(),
                    "Current CharacterLevelDefinitions fetched")
                .WithoutException();

        public static void CharacterLevelDefinitionsFetchingCurrent(
                ILogger logger)
            => _characterLevelDefinitionsFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsFetchingCurrent.ToEventId(),
                    "Fetching current CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsInitialized(
                ILogger logger)
            => _characterLevelDefinitionsInitialized.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsInitialized
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsInitialized.ToEventId(),
                    "CharacterLevelDefinitions initialized")
                .WithoutException();

        public static void CharacterLevelDefinitionsInitializing(
                ILogger logger)
            => _characterLevelDefinitionsInitializing.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsInitializing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsInitializing.ToEventId(),
                    "Initializing CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsNoChangesGiven(
                ILogger logger)
            => _characterLevelDefinitionsNoChangesGiven.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsNoChangesGiven
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    EventType.CharacterLevelDefinitionsNoChangesGiven.ToEventId(),
                    "No changes given for CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsUpdated(
                ILogger logger)
            => _characterLevelDefinitionsUpdated.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsUpdated
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsUpdated.ToEventId(),
                    "CharacterLevelDefinitions updated")
                .WithoutException();

        public static void CharacterLevelDefinitionsUpdating(
                ILogger logger)
            => _characterLevelDefinitionsUpdating.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsUpdating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsUpdating.ToEventId(),
                    "Updating CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionUpdated(
                ILogger logger,
                int level,
                int experienceThreshold)
            => _characterLevelDefinitionUpdated.Invoke(
                logger,
                level,
                experienceThreshold);
        private static readonly Action<ILogger, int, int> _characterLevelDefinitionUpdated
            = LoggerMessage.Define<int, int>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionUpdated.ToEventId(),
                    "CharacterLevelDefinitions updated:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}")
                .WithoutException();

        public static void CharacterLevelDefinitionUpdating(
                ILogger logger,
                int level,
                int experienceThreshold)
            => _characterLevelDefinitionUpdating.Invoke(
                logger,
                level,
                experienceThreshold);
        private static readonly Action<ILogger, int, int> _characterLevelDefinitionUpdating
            = LoggerMessage.Define<int, int>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionUpdating.ToEventId(),
                    "Updating CharacterLevelDefinition:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}")
                .WithoutException();

        public static void CharacterLevelDefinitionValidationFailed(
                ILogger logger,
                int level, 
                decimal experienceThreshold,
                decimal previousExperienceThreshold)
            => _characterLevelDefinitionValidationFailed.Invoke(
                logger,
                level,
                experienceThreshold, 
                previousExperienceThreshold);
        private static readonly Action<ILogger, int, decimal, decimal> _characterLevelDefinitionValidationFailed
            = LoggerMessage.Define<int, decimal, decimal>(
                    LogLevel.Warning,
                    EventType.CharacterLevelDefinitionValidationFailed.ToEventId(),
                    "CharacterLevelDefinition is not valid:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}\r\n\tPreviousExperienceThreshold: {PreviousExperienceThreshold}")
                .WithoutException();

        public static void CharacterLevelsNotInitialized(
                ILogger logger)
            => _characterLevelsNotInitialized.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelsNotInitialized
            = LoggerMessage.Define(
                    LogLevel.Information,
                    EventType.CharacterLevelsNotInitialized.ToEventId(),
                    "CharacterLevelDefinitions not initialized: Definition for Level 1 is missing.")
                .WithoutException();
    }
}
