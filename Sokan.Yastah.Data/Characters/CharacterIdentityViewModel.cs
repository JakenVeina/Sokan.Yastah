using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Characters
{
    public class CharacterIdentityViewModel
    {
        public CharacterIdentityViewModel(
            long id,
            string name,
            UserIdentityViewModel owner)
        {
            Id = id;
            Name = name;
            Owner = owner;
        }

        public long Id { get; }

        public string Name { get; }

        public UserIdentityViewModel Owner { get; }
    }
}
