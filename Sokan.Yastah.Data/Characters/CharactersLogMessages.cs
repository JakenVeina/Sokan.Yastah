using System;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Data.Characters
{
    internal static class CharactersLogMessages
    {
        public enum EventType
        {
            CharacterGuildIdentitiesEnumerating             = DataLogEventType.Characters + 0x0001,
            CharacterGuildVersionsEnumeratingAny            = DataLogEventType.Characters + 0x0002,
            CharacterGuildCreating                          = DataLogEventType.Characters + 0x0003,
            CharacterGuildCreated                           = DataLogEventType.Characters + 0x0004,
            CharacterGuildUpdating                          = DataLogEventType.Characters + 0x0005,
            CharacterGuildUpdated                           = DataLogEventType.Characters + 0x0006,
            CharacterGuildNoChangesGiven                    = DataLogEventType.Characters + 0x0007,
            CharacterGuildVersionNotFound                   = DataLogEventType.Characters + 0x0008,
            CharacterGuildDivisionIdentitiesEnumerating     = DataLogEventType.Characters + 0x0009,
            CharacterGuildDivisionVersionsEnumeratingAny    = DataLogEventType.Characters + 0x000A,
            CharacterGuildDivisionCreating                  = DataLogEventType.Characters + 0x000B,
            CharacterGuildDivisionCreated                   = DataLogEventType.Characters + 0x000C,
            CharacterGuildDivisionUpdating                  = DataLogEventType.Characters + 0x000D,
            CharacterGuildDivisionUpdated                   = DataLogEventType.Characters + 0x000E,
            CharacterGuildDivisionNoChangesGiven            = DataLogEventType.Characters + 0x000F,
            CharacterGuildDivisionVersionNotFound           = DataLogEventType.Characters + 0x0010,
            CharacterLevelDefinitionsEnumeratingAny         = DataLogEventType.Characters + 0x0011,
            CharacterLevelDefinitionsEnumerating            = DataLogEventType.Characters + 0x0012,
            CharacterLevelDefinitionMerging                 = DataLogEventType.Characters + 0x0013,
            CharacterLevelDefinitionMerged                  = DataLogEventType.Characters + 0x0014,
            CharacterLevelDefinitionCreating                = DataLogEventType.Characters + 0x0015,
            CharacterLevelDefinitionNoChangesGiven          = DataLogEventType.Characters + 0x0016,
            CharacterLevelDefinitionVersionCreating         = DataLogEventType.Characters + 0x0017,
            CharacterCreating                               = DataLogEventType.Characters + 0x0018,
            CharacterCreated                                = DataLogEventType.Characters + 0x0019,
            CharacterUpdating                               = DataLogEventType.Characters + 0x001A,
            CharacterUpdated                                = DataLogEventType.Characters + 0x001B,
            CharacterNoChangesGiven                         = DataLogEventType.Characters + 0x001C,
            CharacterVersionNotFound                        = DataLogEventType.Characters + 0x001D
        }

        public static void CharacterCreated(
                ILogger logger,
                long characterId,
                long versionId)
            => _characterCreated.Invoke(
                logger,
                characterId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterCreated.ToEventId(),
                    $"{nameof(CharacterGuildDivisionEntity)} created: {{CharacterId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterCreating(
                ILogger logger,
                ulong ownerId,
                string name,
                long divisionId,
                decimal experiencePoints,
                decimal goldAmount,
                decimal insanityValue,
                long actionId)
            => _characterCreating.Invoke(
                logger,
                ownerId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                actionId);
        private static readonly Action<ILogger, ulong, string, long, decimal, decimal, decimal, long> _characterCreating
            = LoggerMessageEx.Define<ulong, string, long, decimal, decimal, decimal, long>(
                    LogLevel.Information,
                    EventType.CharacterCreating.ToEventId(),
                    $"Creating {nameof(CharacterEntity)}: \r\n\tOwnerId: {{OwnerId}}\r\n\tName: {{Name}}\r\n\tDivisionId: {{DivisionId}}\r\n\tExperiencePoints: {{ExperiencePoints}}\r\n\tGoldAmount: {{GoldAmount}}\r\n\tInsanityValue: {{InsanityValue}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void CharacterGuildCreated(
                ILogger logger,
                long guildId,
                long versionId)
            => _characterGuildCreated.Invoke(
                logger,
                guildId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterGuildCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildCreated.ToEventId(),
                    $"{nameof(CharacterGuildEntity)} created: {{GuildId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterGuildCreating(
                ILogger logger,
                string name,
                long actionId)
            => _characterGuildCreating.Invoke(
                logger,
                name,
                actionId);
        private static readonly Action<ILogger, string, long> _characterGuildCreating
            = LoggerMessage.Define<string, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildCreating.ToEventId(),
                    $"Creating {nameof(CharacterGuildEntity)}: \r\n\tName: {{Name}}]\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionCreated(
                ILogger logger,
                long divisionId,
                long versionId)
            => _characterGuildDivisionCreated.Invoke(
                logger,
                divisionId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterGuildDivisionCreated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionCreated.ToEventId(),
                    $"{nameof(CharacterGuildDivisionEntity)} created: {{DivisionId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionCreating(
                ILogger logger,
                long guildId,
                string name,
                long actionId)
            => _characterGuildDivisionCreating.Invoke(
                logger,
                guildId,
                name,
                actionId);
        private static readonly Action<ILogger, long, string, long> _characterGuildDivisionCreating
            = LoggerMessage.Define<long, string, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionCreating.ToEventId(),
                    $"Creating {nameof(CharacterGuildDivisionEntity)}: \r\n\tGuildId: {{GuildId}}\r\n\tName: {{Name}}]\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionIdentitiesEnumerating(
                ILogger logger,
                Optional<long> guildId,
                Optional<bool> isDeleted)
            => _characterGuildDivisionIdentitiesEnumerating.Invoke(
                logger,
                guildId,
                isDeleted);
        private static readonly Action<ILogger, Optional<long>, Optional<bool>> _characterGuildDivisionIdentitiesEnumerating
            = LoggerMessage.Define<Optional<long>, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(CharacterGuildDivisionIdentityViewModel)}: \r\n\tGuildId: {{GuildId}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterGuildDivisionNoChangesGiven(
                ILogger logger,
                long divisionId)
            => _characterGuildDivisionNoChangesGiven.Invoke(
                logger,
                divisionId);
        private static readonly Action<ILogger, long> _characterGuildDivisionNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionNoChangesGiven.ToEventId(),
                    $"No changes given for {nameof(CharacterGuildDivisionVersionEntity)}: {{DivisionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionUpdated(
                ILogger logger,
                long divisionId,
                long versionId)
            => _characterGuildDivisionUpdated.Invoke(
                logger,
                divisionId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterGuildDivisionUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionUpdated.ToEventId(),
                    $"{nameof(CharacterGuildDivisionEntity)} updated: {{DivisionId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionUpdating(
                ILogger logger,
                long divisionId,
                long actionId,
                Optional<string> name,
                Optional<bool> isDeleted)
            => _characterGuildDivisionUpdating.Invoke(
                logger,
                divisionId,
                actionId,
                name,
                isDeleted);
        private static readonly Action<ILogger, long, long, Optional<string>, Optional<bool>> _characterGuildDivisionUpdating
            = LoggerMessage.Define<long, long, Optional<string>, Optional<bool>>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionUpdating.ToEventId(),
                    $"Updating {nameof(CharacterGuildDivisionEntity)}: \r\n\tDivisionId: {{DivisionId}}\r\n\tActionId: {{ActionId}}\r\n\tName: {{Name}}]\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterGuildDivisionVersionNotFound(
                ILogger logger,
                long divisionId)
            => _characterGuildDivisionVersionNotFound.Invoke(
                logger,
                divisionId);
        private static readonly Action<ILogger, long> _characterGuildDivisionVersionNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterGuildDivisionVersionNotFound.ToEventId(),
                    $"{nameof(CharacterGuildDivisionVersionEntity)} not found: DivisionId: {{DivisionId}}")
                .WithoutException();

        public static void CharacterGuildDivisionVersionsEnumeratingAny(
                ILogger logger,
                Optional<long> guildId,
                Optional<IEnumerable<long>> excludedDivisionIds,
                Optional<string> name,
                Optional<bool> isDeleted,
                Optional<bool> isLatestVersion)
            => _characterGuildDivisionVersionsEnumeratingAny.Invoke(
                logger,
                guildId,
                excludedDivisionIds,
                name,
                isDeleted,
                isLatestVersion);
        private static readonly Action<ILogger, Optional<long>, Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>> _characterGuildDivisionVersionsEnumeratingAny
            = LoggerMessage.Define<Optional<long>, Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterGuildDivisionVersionsEnumeratingAny.ToEventId(),
                    $"Enumerating for any {nameof(CharacterGuildDivisionVersionEntity)}: \r\n\tGuildId: {{GuildId}}\r\n\tExcludedDivisionIds: {{ExcludedDivisionIds}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}\r\n\tIsLatestVersion: {{IsLatestVersion}}")
                .WithoutException();

        public static void CharacterGuildIdentitiesEnumerating(
                ILogger logger,
                Optional<bool> isDeleted)
            => _characterGuildIdentitiesEnumerating.Invoke(
                logger,
                isDeleted);
        private static readonly Action<ILogger, Optional<bool>> _characterGuildIdentitiesEnumerating
            = LoggerMessage.Define<Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterGuildIdentitiesEnumerating.ToEventId(),
                    $"Enumerating {nameof(CharacterGuildIdentityViewModel)}: \r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterGuildNoChangesGiven(
                ILogger logger,
                long guildId)
            => _characterGuildNoChangesGiven.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterGuildNoChangesGiven.ToEventId(),
                    $"No changes given for {nameof(CharacterGuildVersionEntity)}: {{GuildId}}")
                .WithoutException();

        public static void CharacterGuildUpdated(
                ILogger logger,
                long guildId,
                long versionId)
            => _characterGuildUpdated.Invoke(
                logger,
                guildId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterGuildUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterGuildUpdated.ToEventId(),
                    $"{nameof(CharacterGuildEntity)} updated: {{GuildId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterGuildUpdating(
                ILogger logger,
                long guildId,
                long actionId,
                Optional<string> name,
                Optional<bool> isDeleted)
            => _characterGuildUpdating.Invoke(
                logger,
                guildId,
                actionId,
                name,
                isDeleted);
        private static readonly Action<ILogger, long, long, Optional<string>, Optional<bool>> _characterGuildUpdating
            = LoggerMessage.Define<long, long, Optional<string>, Optional<bool>>(
                    LogLevel.Information,
                    EventType.CharacterGuildUpdating.ToEventId(),
                    $"Updating {nameof(CharacterGuildEntity)}: \r\n\tId: {{Id}}\r\n\tActionId: {{ActionId}}\r\n\tName: {{Name}}]\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterGuildVersionNotFound(
                ILogger logger,
                long guildId)
            => _characterGuildVersionNotFound.Invoke(
                logger,
                guildId);
        private static readonly Action<ILogger, long> _characterGuildVersionNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterGuildVersionNotFound.ToEventId(),
                    $"{nameof(CharacterGuildVersionEntity)} not found: GuildId: {{GuildId}}")
                .WithoutException();

        public static void CharacterGuildVersionsEnumeratingAny(
                ILogger logger,
                Optional<long> guildId,
                Optional<IEnumerable<long>> excludedGuildIds,
                Optional<string> name,
                Optional<bool> isDeleted,
                Optional<bool> isLatestVersion)
            => _characterGuildVersionsEnumeratingAny.Invoke(
                logger,
                guildId,
                excludedGuildIds,
                name,
                isDeleted,
                isLatestVersion);
        private static readonly Action<ILogger, Optional<long>, Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>> _characterGuildVersionsEnumeratingAny
            = LoggerMessage.Define<Optional<long>, Optional<IEnumerable<long>>, Optional<string>, Optional<bool>, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterGuildVersionsEnumeratingAny.ToEventId(),
                    $"Enumerating for any {nameof(CharacterGuildVersionEntity)}: \r\n\tGuildId: {{GuildId}}\r\n\tExcludedGuildIds: {{ExcludedGuildIds}}\r\n\tName: {{Name}}\r\n\tIsDeleted: {{IsDeleted}}\r\n\tIsLatestVersion: {{IsLatestVersion}}")
                .WithoutException();

        public static void CharacterLevelDefinitionCreating(
                ILogger logger,
                int level)
            => _characterLevelDefinitionCreating.Invoke(
                logger,
                level);
        private static readonly Action<ILogger, int> _characterLevelDefinitionCreating
            = LoggerMessage.Define<int>(
                    LogLevel.Information,
                    EventType.CharacterLevelDefinitionCreating.ToEventId(),
                    $"Creating {nameof(CharacterLevelDefinitionEntity)}: {{Level}}")
                .WithoutException();

        public static void CharacterLevelDefinitionMerged(
                ILogger logger,
                int level,
                long versionId)
            => _characterLevelDefinitionMerged.Invoke(
                logger,
                level,
                versionId);
        private static readonly Action<ILogger, int, long> _characterLevelDefinitionMerged
            = LoggerMessage.Define<int, long>(
                    LogLevel.Information,
                    EventType.CharacterLevelDefinitionMerged.ToEventId(),
                    $"{nameof(CharacterLevelDefinitionEntity)} merged: {{Level}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterLevelDefinitionMerging(
                ILogger logger,
                int level,
                int experienceThreshold,
                bool isDeleted,
                long actionId)
            => _characterLevelDefinitionMerging.Invoke(
                logger,
                level,
                experienceThreshold,
                isDeleted,
                actionId);
        private static readonly Action<ILogger, int, int, bool, long> _characterLevelDefinitionMerging
            = LoggerMessage.Define<int, int, bool, long>(
                    LogLevel.Information,
                    EventType.CharacterLevelDefinitionMerging.ToEventId(),
                    $"Merging {nameof(CharacterLevelDefinitionEntity)}: \r\n\tLevel: {{Level}}\r\n\tExperienceThreshold: {{ExperienceThreshold}}\r\n\tIsDeleted {{IsDeleted}}\r\n\tActionId: {{ActionId}}")
                .WithoutException();

        public static void CharacterLevelDefinitionNoChangesGiven(
                ILogger logger,
                int level,
                long versionId)
            => _characterLevelDefinitionNoChangesGiven.Invoke(
                logger,
                level,
                versionId);
        private static readonly Action<ILogger, int, long> _characterLevelDefinitionNoChangesGiven
            = LoggerMessage.Define<int, long>(
                    LogLevel.Information,
                    EventType.CharacterLevelDefinitionNoChangesGiven.ToEventId(),
                    $"No changes given for {nameof(CharacterLevelDefinitionVersionEntity)}: \r\n\tLevel: {{Level}}\r\n\tVersionId: {{VersionId}}")
                .WithoutException();

        public static void CharacterLevelDefinitionsEnumerating(
                ILogger logger,
                Optional<bool> isDeleted)
            => _characterLevelDefinitionsEnumerating.Invoke(
                logger,
                isDeleted);
        private static readonly Action<ILogger, Optional<bool>> _characterLevelDefinitionsEnumerating
            = LoggerMessage.Define<Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsEnumerating.ToEventId(),
                    $"Enumerating {nameof(CharacterLevelDefinitionViewModel)}: \r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterLevelDefinitionsEnumeratingAny(
                ILogger logger,
                Optional<int> level,
                Optional<int> experienceThreshold,
                Optional<bool> isDeleted)
            => _characterLevelDefinitionsEnumeratingAny.Invoke(
                logger,
                level,
                experienceThreshold,
                isDeleted);
        private static readonly Action<ILogger, Optional<int>, Optional<int>, Optional<bool>> _characterLevelDefinitionsEnumeratingAny
            = LoggerMessage.Define<Optional<int>, Optional<int>, Optional<bool>>(
                    LogLevel.Debug,
                    EventType.CharacterLevelDefinitionsEnumeratingAny.ToEventId(),
                    $"Enumerating for any {nameof(CharacterLevelDefinitionEntity)}: \r\n\tLevel: {{Level}}\r\n\tExperienceThreshold: {{ExperienceThreshold}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterLevelDefinitionVersionCreating(
                ILogger logger,
                int level)
            => _characterLevelDefinitionVersionCreating.Invoke(
                logger,
                level);
        private static readonly Action<ILogger, int> _characterLevelDefinitionVersionCreating
            = LoggerMessage.Define<int>(
                    LogLevel.Information,
                    EventType.CharacterLevelDefinitionVersionCreating.ToEventId(),
                    $"Creating {nameof(CharacterLevelDefinitionVersionEntity)}: {{Level}}")
                .WithoutException();

        public static void CharacterNoChangesGiven(
                ILogger logger,
                long characterId)
            => _characterNoChangesGiven.Invoke(
                logger,
                characterId);
        private static readonly Action<ILogger, long> _characterNoChangesGiven
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterNoChangesGiven.ToEventId(),
                    $"No changes given for {nameof(CharacterVersionEntity)}: {{CharacterId}}")
                .WithoutException();

        public static void CharacterUpdated(
                ILogger logger,
                long characterId,
                long versionId)
            => _characterUpdated.Invoke(
                logger,
                characterId,
                versionId);
        private static readonly Action<ILogger, long, long> _characterUpdated
            = LoggerMessage.Define<long, long>(
                    LogLevel.Information,
                    EventType.CharacterUpdated.ToEventId(),
                    $"{nameof(CharacterGuildDivisionEntity)} created: {{CharacterId}}, version {{VersionId}}")
                .WithoutException();

        public static void CharacterUpdating(
                ILogger logger,
                long characterId,
                long actionId,
                Optional<string> name,
                Optional<long> divisionId,
                Optional<decimal> experiencePoints,
                Optional<decimal> goldAmount,
                Optional<decimal> insanityValue,
                Optional<bool> isDeleted)
            => _characterUpdating.Invoke(
                logger,
                characterId,
                actionId,
                name,
                divisionId,
                experiencePoints,
                goldAmount,
                insanityValue,
                isDeleted);
        private static readonly Action<ILogger, long, long, Optional<string>, Optional<long>, Optional<decimal>, Optional<decimal>, Optional<decimal>, Optional<bool>> _characterUpdating
            = LoggerMessageEx.Define<long, long, Optional<string>, Optional<long>, Optional<decimal>, Optional<decimal>, Optional<decimal>, Optional<bool>>(
                    LogLevel.Information,
                    EventType.CharacterUpdating.ToEventId(),
                    $"Updating {nameof(CharacterEntity)}: \r\n\tCharacterId: {{CharacterId}}\r\n\tActionId: {{ActionId}}\r\n\tName: {{Name}}\r\n\tDivisionId: {{DivisionId}}\r\n\tExperiencePoints: {{ExperiencePoints}}\r\n\tGoldAmount: {{GoldAmount}}\r\n\tInsanityValue: {{InsanityValue}}\r\n\tIsDeleted: {{IsDeleted}}")
                .WithoutException();

        public static void CharacterVersionNotFound(
                ILogger logger,
                long characterId)
            => _characterVersionNotFound.Invoke(
                logger,
                characterId);
        private static readonly Action<ILogger, long> _characterVersionNotFound
            = LoggerMessage.Define<long>(
                    LogLevel.Information,
                    EventType.CharacterVersionNotFound.ToEventId(),
                    $"{nameof(CharacterVersionEntity)} not found: CharacterId: {{CharacterId}}")
                .WithoutException();
    }
}
