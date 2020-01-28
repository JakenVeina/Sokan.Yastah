using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    internal class CharactersTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        private static readonly CharactersTestEntitySetBuilder Instance
            = new CharactersTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance.Build();

        private CharactersTestEntitySetBuilder() { }

        protected override IReadOnlyList<CharacterEntity>? CreateCharacters()
            => Enumerable.Empty<CharacterEntity>()
                .Append(new CharacterEntity(    id: 1,  ownerId: 1  ))
                .Append(new CharacterEntity(    id: 2,  ownerId: 2  ))
                .Append(new CharacterEntity(    id: 3,  ownerId: 3  ))
                .Append(new CharacterEntity(    id: 4,  ownerId: 1  ))
                .ToArray();

        protected override IReadOnlyList<CharacterVersionEntity>? CreateCharacterVersions()
            => Enumerable.Empty<CharacterVersionEntity>()
                .Append(new CharacterVersionEntity( id: 1,  characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 74, previousVersionId: null,    nextVersionId: 12   ))
                .Append(new CharacterVersionEntity( id: 2,  characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 75, previousVersionId: null,    nextVersionId: 9    ))
                .Append(new CharacterVersionEntity( id: 3,  characterId: 3, name: "Character 3",    divisionId: 3,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 76, previousVersionId: null,    nextVersionId: 4    ))
                .Append(new CharacterVersionEntity( id: 4,  characterId: 3, name: "Character 3a",   divisionId: 1,  experiencePoints: 100,  goldAmount: 900,    insanityValue: 9,     isDeleted: false,   creationId: 77, previousVersionId: 3,       nextVersionId: 5    ))
                .Append(new CharacterVersionEntity( id: 5,  characterId: 3, name: "Character 3a",   divisionId: 1,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 78, previousVersionId: 4,       nextVersionId: 6    ))
                .Append(new CharacterVersionEntity( id: 6,  characterId: 3, name: "Character 3a",   divisionId: 2,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 79, previousVersionId: 5,       nextVersionId: 7    ))
                .Append(new CharacterVersionEntity( id: 7,  characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 200,  goldAmount: 850,    insanityValue: 9,     isDeleted: false,   creationId: 80, previousVersionId: 6,       nextVersionId: 8    ))
                .Append(new CharacterVersionEntity( id: 8,  characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 250,  goldAmount: 750,    insanityValue: 10,    isDeleted: false,   creationId: 81, previousVersionId: 7,       nextVersionId: 14   ))
                .Append(new CharacterVersionEntity( id: 9,  characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 100,  goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 82, previousVersionId: 2,       nextVersionId: 10   ))
                .Append(new CharacterVersionEntity( id: 10, characterId: 2, name: "Character 2",    divisionId: 3,  experiencePoints: 250,  goldAmount: 400,    insanityValue: 10,    isDeleted: false,   creationId: 83, previousVersionId: 9,       nextVersionId: 11   ))
                .Append(new CharacterVersionEntity( id: 11, characterId: 2, name: "Character 2a",   divisionId: 3,  experiencePoints: 250,  goldAmount: 400,    insanityValue: 10,    isDeleted: false,   creationId: 84, previousVersionId: 10,      nextVersionId: 13   ))
                .Append(new CharacterVersionEntity( id: 12, characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 300,  goldAmount: 1200,   insanityValue: 10,    isDeleted: false,   creationId: 85, previousVersionId: 1,       nextVersionId: 15   ))
                .Append(new CharacterVersionEntity( id: 13, characterId: 2, name: "Character 2a",   divisionId: 3,  experiencePoints: 550,  goldAmount: 600,    insanityValue: 10,    isDeleted: false,   creationId: 86, previousVersionId: 11,      nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 14, characterId: 3, name: "Character 3b",   divisionId: 2,  experiencePoints: 550,  goldAmount: 950,    insanityValue: 10,    isDeleted: false,   creationId: 87, previousVersionId: 8,       nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 15, characterId: 1, name: "Character 1",    divisionId: 1,  experiencePoints: 300,  goldAmount: 1200,   insanityValue: 10,    isDeleted: true,    creationId: 88, previousVersionId: 12,      nextVersionId: null ))
                .Append(new CharacterVersionEntity( id: 16, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 89, previousVersionId: null,    nextVersionId: 17   ))
                .Append(new CharacterVersionEntity( id: 17, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: true,    creationId: 90, previousVersionId: 16,      nextVersionId: 18   ))
                .Append(new CharacterVersionEntity( id: 18, characterId: 4, name: "Character 4",    divisionId: 1,  experiencePoints: 0,    goldAmount: 1000,   insanityValue: 10,    isDeleted: false,   creationId: 91, previousVersionId: 17,      nextVersionId: null ))
                .ToArray();
    }
}
