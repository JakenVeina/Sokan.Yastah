using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    internal class CharacterGuildDivisionsTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        private static readonly CharacterGuildDivisionsTestEntitySetBuilder Instance
            = new CharacterGuildDivisionsTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance.Build();

        private CharacterGuildDivisionsTestEntitySetBuilder() { }

        protected override IReadOnlyList<CharacterGuildDivisionEntity>? CreateCharacterGuildDivisions()
            => Enumerable.Empty<CharacterGuildDivisionEntity>()
                .Append(new CharacterGuildDivisionEntity(   id: 1,  guildId: 1  ))
                .Append(new CharacterGuildDivisionEntity(   id: 2,  guildId: 2  ))
                .Append(new CharacterGuildDivisionEntity(   id: 3,  guildId: 2  ))
                .Append(new CharacterGuildDivisionEntity(   id: 4,  guildId: 1  ))
                .Append(new CharacterGuildDivisionEntity(   id: 5,  guildId: 3  ))
                .Append(new CharacterGuildDivisionEntity(   id: 6,  guildId: 2  ))
                .Append(new CharacterGuildDivisionEntity(   id: 7,  guildId: 3  ))
                .Append(new CharacterGuildDivisionEntity(   id: 8,  guildId: 1  ))
                .Append(new CharacterGuildDivisionEntity(   id: 9,  guildId: 3  ))
                .ToArray();

        protected override IReadOnlyList<CharacterGuildDivisionVersionEntity> CreateCharacterGuildDivisionVersions()
            => Enumerable.Empty<CharacterGuildDivisionVersionEntity>()
                .Append(new CharacterGuildDivisionVersionEntity(    id: 1,  divisionId: 1,  name: "Character Guild 1, Division 1",  isDeleted: false,   creationId: 49, previousVersionId: null, nextVersionId: 6       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 2,  divisionId: 2,  name: "Character Guild 2, Division 1",  isDeleted: false,   creationId: 50, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 3,  divisionId: 3,  name: "Character Guild 2, Division 2",  isDeleted: false,   creationId: 51, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 4,  divisionId: 4,  name: "Character Guild 1, Division 2",  isDeleted: false,   creationId: 52, previousVersionId: null, nextVersionId: 5       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 5,  divisionId: 4,  name: "Character Guild 1, Division 2",  isDeleted: true,    creationId: 53, previousVersionId: 4,    nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 6,  divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: false,   creationId: 54, previousVersionId: 1,    nextVersionId: 10      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 7,  divisionId: 5,  name: "Character Guild 3, Division 1",  isDeleted: false,   creationId: 55, previousVersionId: null, nextVersionId: 8       ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 8,  divisionId: 5,  name: "Character Guild 3, Division 1a", isDeleted: false,   creationId: 56, previousVersionId: 7,    nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 9,  divisionId: 6,  name: "Character Guild 2, Division 3",  isDeleted: false,   creationId: 57, previousVersionId: null, nextVersionId: 17      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 10, divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: true,    creationId: 58, previousVersionId: 6,    nextVersionId: 14      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 11, divisionId: 7,  name: "Character Guild 3, Division 2",  isDeleted: false,   creationId: 59, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 12, divisionId: 8,  name: "Character Guild 1, Division 3",  isDeleted: false,   creationId: 60, previousVersionId: null, nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 13, divisionId: 9,  name: "Character Guild 3, Division 3",  isDeleted: false,   creationId: 61, previousVersionId: null, nextVersionId: 15      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 14, divisionId: 1,  name: "Character Guild 1, Division 1a", isDeleted: false,   creationId: 62, previousVersionId: 10,   nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 15, divisionId: 9,  name: "Character Guild 3, Division 3a", isDeleted: false,   creationId: 63, previousVersionId: 13,   nextVersionId: 16      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 16, divisionId: 9,  name: "Character Guild 3, Division 3b", isDeleted: false,   creationId: 64, previousVersionId: 15,   nextVersionId: null    ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 17, divisionId: 6,  name: "Character Guild 2, Division 3a", isDeleted: false,   creationId: 65, previousVersionId: 9,    nextVersionId: 18      ))
                .Append(new CharacterGuildDivisionVersionEntity(    id: 18, divisionId: 6,  name: "Character Guild 2, Division 3",  isDeleted: false,   creationId: 66, previousVersionId: 17,   nextVersionId: null    ))
                .ToArray();
    }
}
