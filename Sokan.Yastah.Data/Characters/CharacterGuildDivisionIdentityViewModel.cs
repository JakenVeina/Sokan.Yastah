using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Characters
{
    public class CharacterGuildDivisionIdentityViewModel
    {
        public CharacterGuildDivisionIdentityViewModel(
            long id,
            string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; }

        public string Name { get; }

        internal static readonly Expression<Func<CharacterGuildDivisionVersionEntity, CharacterGuildDivisionIdentityViewModel>> FromVersionEntityProjection
            = ve => new CharacterGuildDivisionIdentityViewModel(
                ve.DivisionId,
                ve.Name);
    }
}
