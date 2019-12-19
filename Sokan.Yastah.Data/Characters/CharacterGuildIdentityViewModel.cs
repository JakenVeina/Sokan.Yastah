using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Characters
{
    public class CharacterGuildIdentityViewModel
    {
        public CharacterGuildIdentityViewModel(
            long id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }

        public string Name { get; }

        internal static readonly Expression<Func<CharacterGuildVersionEntity, CharacterGuildIdentityViewModel>> FromVersionEntityProjection
            = ve => new CharacterGuildIdentityViewModel(
                ve.GuildId,
                ve.Name);
    }
}
