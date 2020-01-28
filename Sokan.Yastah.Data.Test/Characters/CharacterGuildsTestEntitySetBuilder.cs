using System.Collections.Generic;
using System.Linq;

using Sokan.Yastah.Data.Characters;

namespace Sokan.Yastah.Data.Test.Characters
{
    internal class CharacterGuildsTestEntitySetBuilder
        : YastahTestEntitySetBuilder
    {
        private static readonly CharacterGuildsTestEntitySetBuilder Instance
            = new CharacterGuildsTestEntitySetBuilder();

        public static readonly YastahTestEntitySet SharedSet
            = NewSet();

        public static YastahTestEntitySet NewSet()
            => Instance.Build();

        private CharacterGuildsTestEntitySetBuilder() { }

        protected override IReadOnlyList<CharacterGuildEntity>? CreateCharacterGuilds()
            => Enumerable.Empty<CharacterGuildEntity>()
                .Append(new CharacterGuildEntity(   id: 1   ))
                .Append(new CharacterGuildEntity(   id: 2   ))
                .Append(new CharacterGuildEntity(   id: 3   ))
                .ToArray();

        protected override IReadOnlyList<CharacterGuildVersionEntity>? CreateCharacterGuildVersions()
            => Enumerable.Empty<CharacterGuildVersionEntity>()
                .Append(new CharacterGuildVersionEntity(    id: 1,  guildId: 1, name: "Character Guild 1",  isDeleted: false,   creationId: 40, previousVersionId: null,    nextVersionId: 2    ))
                .Append(new CharacterGuildVersionEntity(    id: 2,  guildId: 1, name: "Character Guild 1a", isDeleted: false,   creationId: 41, previousVersionId: 1,       nextVersionId: 9    ))
                .Append(new CharacterGuildVersionEntity(    id: 3,  guildId: 2, name: "Character Guild 2",  isDeleted: false,   creationId: 42, previousVersionId: null,    nextVersionId: 5    ))
                .Append(new CharacterGuildVersionEntity(    id: 4,  guildId: 3, name: "Character Guild 3",  isDeleted: false,   creationId: 43, previousVersionId: null,    nextVersionId: 6    ))
                .Append(new CharacterGuildVersionEntity(    id: 5,  guildId: 2, name: "Character Guild 2",  isDeleted: true,    creationId: 44, previousVersionId: 3,       nextVersionId: null ))
                .Append(new CharacterGuildVersionEntity(    id: 6,  guildId: 3, name: "Character Guild 3a", isDeleted: false,   creationId: 45, previousVersionId: 4,       nextVersionId: 7    ))
                .Append(new CharacterGuildVersionEntity(    id: 7,  guildId: 3, name: "Character Guild 3a", isDeleted: true,    creationId: 46, previousVersionId: 6,       nextVersionId: 8    ))
                .Append(new CharacterGuildVersionEntity(    id: 8,  guildId: 3, name: "Character Guild 3a", isDeleted: false,   creationId: 47, previousVersionId: 7,       nextVersionId: null ))
                .Append(new CharacterGuildVersionEntity(    id: 9,  guildId: 1, name: "Character Guild 1",  isDeleted: false,   creationId: 48, previousVersionId: 2,       nextVersionId: null ))
                .ToArray();
    }
}
