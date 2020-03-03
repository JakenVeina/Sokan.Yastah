using System;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Business.Characters
{
    internal static class CharactersLogMessages
    {
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
                    new EventId(2001, nameof(CharacterGuildUpdateFailed)),
                    "CharacterGuild update failed:\r\n\tGuildId: {GuildId}\r\n\tUpdateResult: {UpdateResult}")
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
                    new EventId(2002, nameof(CharacterGuildDeleteFailed)),
                    "CharacterGuild update failed:\r\n\tGuildId: {GuildId}\r\n\tDeleteResult: {DeleteResult}")
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
                    new EventId(2003, nameof(CharacterGuildDivisionUpdateFailed)),
                    "CharacterGuildDivision update failed:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tUpdateResult: {UpdateResult}")
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
                    new EventId(2004, nameof(CharacterGuildDivisionDeleteFailed)),
                    "CharacterGuildDivision update failed:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tDeleteResult: {DeleteResult}")
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
                    new EventId(2005, nameof(CharacterGuildIdValidationFailed)),
                    "CharacterGuild Id is not valid: {GuildId}\r\n\tValidationResult: {ValidationResult}")
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
                    new EventId(2006, nameof(CharacterGuildNameValidationFailed)),
                    "CharacterGuild Name is not valid: {GuildName}\r\n\tValidationResult: {ValidationResult}")
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
                    new EventId(2007, nameof(CharacterGuildDivisionNameValidationFailed)),
                    "CharacterGuildDivision Name is not valid: {DivisionName}\r\n\tValidationResult: {ValidationResult}")
                .WithoutException();

        public static void CharacterLevelDefinitionsNoChangesGiven(
                ILogger logger)
            => _characterLevelDefinitionsNoChangesGiven.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsNoChangesGiven
            = LoggerMessage.Define(
                    LogLevel.Warning,
                    new EventId(2008, nameof(CharacterLevelDefinitionsNoChangesGiven)),
                    "No changes given for CharacterLevelDefinitions")
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
                    new EventId(2009, nameof(CharacterLevelDefinitionValidationFailed)),
                    "CharacterLevelDefinition is not valid:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}\r\n\tPreviousExperienceThreshold: {PreviousExperienceThreshold}")
                .WithoutException();

        public static void CharacterLevelsNotInitialized(
                ILogger logger)
            => _characterLevelsNotInitialized.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelsNotInitialized
            = LoggerMessage.Define(
                    LogLevel.Information,
                    new EventId(3001, nameof(CharacterLevelsNotInitialized)),
                    "CharacterLevelDefinitions not initialized: Definition for Level 1 is missing.")
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
                    new EventId(4001, nameof(CharacterGuildCreating)),
                    "Creating CharacterGuild:\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
                .WithoutException();

        public static void CharacterGuildCreated(
                ILogger logger,
                long guildId)
            => _characterGuildCreated.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildCreated
            = LoggerMessage.Define<long>(
                    LogLevel.Debug,
                    new EventId(4002, nameof(CharacterGuildCreated)),
                    "CharacterGuild deleted:\r\n\tGuildId: {GuildId}")
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
                    new EventId(4003, nameof(CharacterGuildUpdating)),
                    "Updating CharacterGuild:\r\n\tGuildId: {GuildId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4004, nameof(CharacterGuildUpdated)),
                    "CharacterGuild updated:\r\n\tGuildId: {GuildId}")
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
                    new EventId(4005, nameof(CharacterGuildDeleting)),
                    "Deleting CharacterGuild:\r\n\tGuildId: {GuildId}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4006, nameof(CharacterGuildDeleted)),
                    "CharacterGuild deleted:\r\n\tGuildId: {GuildId}")
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
                    new EventId(4007, nameof(CharacterGuildDivisionCreating)),
                    "Creating CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tCreationModel: {CreationModel}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4008, nameof(CharacterGuildDivisionCreated)),
                    "CharacterGuildDivision deleted:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
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
                    new EventId(4009, nameof(CharacterGuildDivisionUpdating)),
                    "Updating CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tUpdateModel: {UpdateModel}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4010, nameof(CharacterGuildDivisionUpdated)),
                    "CharacterGuildDivision updated:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
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
                    new EventId(4011, nameof(CharacterGuildDivisionDeleting)),
                    "Deleting CharacterGuildDivision:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}\r\n\tPerformedById: {PerformedById}")
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
                    new EventId(4012, nameof(CharacterGuildDivisionDeleted)),
                    "CharacterGuildDivision deleted:\r\n\tGuildId: {GuildId}\r\n\tDivisionId: {DivisionId}")
                .WithoutException();

        public static void CharacterLevelDefinitionsInitializing(
                ILogger logger)
            => _characterLevelDefinitionsInitializing.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsInitializing
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4013, nameof(CharacterLevelDefinitionsInitializing)),
                    "Initializing CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsInitialized(
                ILogger logger)
            => _characterLevelDefinitionsInitialized.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsInitialized
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4014, nameof(CharacterLevelDefinitionsInitialized)),
                    "CharacterLevelDefinitions initialized")
                .WithoutException();

        public static void CharacterLevelDefinitionsUpdating(
                ILogger logger)
            => _characterLevelDefinitionsUpdating.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsUpdating
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4015, nameof(CharacterLevelDefinitionsUpdating)),
                    "Updating CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsUpdated(
                ILogger logger)
            => _characterLevelDefinitionsUpdated.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsUpdated
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4016, nameof(CharacterLevelDefinitionsUpdated)),
                    "CharacterLevelDefinitions updated")
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
                    new EventId(4017, nameof(CharacterLevelDefinitionProposed)),
                    "CharacterLevelDefinition proposed:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}\r\n\tPreviousExperienceThreshold: {PreviousExperienceThreshold}")
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
                    new EventId(4018, nameof(CharacterLevelDefinitionUpdating)),
                    "Updating CharacterLevelDefinition:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}")
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
                    new EventId(4019, nameof(CharacterLevelDefinitionUpdated)),
                    "CharacterLevelDefinitions updated:\r\n\tLevel: {Level}\r\n\tExperienceThreshold: {ExperienceThreshold}")
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
                    new EventId(4020, nameof(CharacterLevelDefinitionDeleting)),
                    "Deleting CharacterLevelDefinition:\r\n\tLevel: {Level}")
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
                    new EventId(4021, nameof(CharacterLevelDefinitionDeleted)),
                    "CharacterLevelDefinitions deleted:\r\n\tLevel: {Level}")
                .WithoutException();

        public static void CharacterLevelDefinition1Created(
                ILogger logger)
            => _characterLevelDefinition1Created.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinition1Created
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4022, nameof(CharacterLevelDefinition1Created)),
                    "CharacterLevelDefinition created for Level 1")
                .WithoutException();

        public static void CharacterLevelDefinitionsCacheCleared(
                ILogger logger)
            => _characterLevelDefinitionsCacheCleared.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsCacheCleared
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4023, nameof(CharacterLevelDefinitionsCacheCleared)),
                    "CharacterLevelDefinitions cache cleared")
                .WithoutException();

        public static void CharacterGuildIdentitiesFetchingCurrent(
                ILogger logger)
            => _characterGuildIdentitiesFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterGuildIdentitiesFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4024, nameof(CharacterGuildIdentitiesFetchingCurrent)),
                    "Fetching current CharacterGuild identities")
                .WithoutException();

        public static void CharacterGuildIdentitiesFetchedCurrent(
                ILogger logger)
            => _characterGuildIdentitiesFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterGuildIdentitiesFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4025, nameof(CharacterGuildIdentitiesFetchedCurrent)),
                    "Current CharacterGuildDivision identities fetched")
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
                    new EventId(4026, nameof(CharacterGuildDivisionIdentitiesFetchingCurrent)),
                    "Fetching current CharacterGuildDivision identities:\r\n\tGuildId: {GuildId}")
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
                    new EventId(4027, nameof(CharacterGuildDivisionIdentitiesFetchedCurrent)),
                    "Current CharacterGuildDivision identities fetched:\r\n\tGuildId: {GuildId}")
                .WithoutException();

        public static void CharacterLevelDefinitionsFetchingCurrent(
                ILogger logger)
            => _characterLevelDefinitionsFetchingCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsFetchingCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4028, nameof(CharacterLevelDefinitionsFetchingCurrent)),
                    "Fetching current CharacterLevelDefinitions")
                .WithoutException();

        public static void CharacterLevelDefinitionsFetchedCurrent(
                ILogger logger)
            => _characterLevelDefinitionsFetchedCurrent.Invoke(
                logger);
        private static readonly Action<ILogger> _characterLevelDefinitionsFetchedCurrent
            = LoggerMessage.Define(
                    LogLevel.Debug,
                    new EventId(4029, nameof(CharacterLevelDefinitionsFetchedCurrent)),
                    "Current CharacterLevelDefinitions fetched")
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
                    new EventId(4030, nameof(CharacterGuildIdValidationSucceeded)),
                    "CharacterGuild Id is valid: {GuildId}")
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
                    new EventId(4031, nameof(CharacterGuildNameValidationSucceeded)),
                    "CharacterGuild Name is valid: {GuildName}")
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
                    new EventId(4032, nameof(CharacterGuildDivisionNameValidationSucceeded)),
                    "CharacterGuildDivision Name is valid: {DivisionName}")
                .WithoutException();
    }
}
