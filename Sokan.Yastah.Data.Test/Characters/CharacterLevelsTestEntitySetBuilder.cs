using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    internal class CharacterLevelsTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        private static readonly CharacterLevelsTestEntitySetBuilder Instance
            = new CharacterLevelsTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance
                .Build();

        private CharacterLevelsTestEntitySetBuilder() { }

        protected override IReadOnlyList<CharacterLevelDefinitionEntity>? CreateCharacterLevelDefinitions()
            => Enumerable.Empty<CharacterLevelDefinitionEntity>()
                .Append(new CharacterLevelDefinitionEntity( level: 1    ))
                .Append(new CharacterLevelDefinitionEntity( level: 2    ))
                .Append(new CharacterLevelDefinitionEntity( level: 3    ))
                .ToArray();

        protected override IReadOnlyList<CharacterLevelDefinitionVersionEntity>? CreateCharacterLevelDefinitionVersions()
            => Enumerable.Empty<CharacterLevelDefinitionVersionEntity>()
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 1,  level: 1,   experienceThreshold: 0,     isDeleted: false,   creationId: 67L,    previousVersionId: null,    nextVersionId: null ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 2,  level: 2,   experienceThreshold: 10,    isDeleted: false,   creationId: 68L,    previousVersionId: null,    nextVersionId: 4L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 3,  level: 3,   experienceThreshold: 20,    isDeleted: false,   creationId: 68L,    previousVersionId: null,    nextVersionId: 6L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 4,  level: 2,   experienceThreshold: 11,    isDeleted: false,   creationId: 69L,    previousVersionId: 1L,      nextVersionId: 6L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 5,  level: 3,   experienceThreshold: 20,    isDeleted: true,    creationId: 70L,    previousVersionId: 3L,      nextVersionId: null ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 6,  level: 2,   experienceThreshold: 11,    isDeleted: true,    creationId: 71L,    previousVersionId: 4L,      nextVersionId: 7L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 7,  level: 2,   experienceThreshold: 11,    isDeleted: false,   creationId: 72L,    previousVersionId: 6L,      nextVersionId: 8L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 8,  level: 2,   experienceThreshold: 21,    isDeleted: false,   creationId: 73L,    previousVersionId: 7L,      nextVersionId: 9L   ))
                .Append(new CharacterLevelDefinitionVersionEntity(  id: 9,  level: 2,   experienceThreshold: 31,    isDeleted: false,   creationId: 74L,    previousVersionId: 8L,      nextVersionId: null ))
                .ToArray();
    }
}
