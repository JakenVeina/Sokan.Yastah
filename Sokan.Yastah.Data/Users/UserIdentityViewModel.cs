namespace Sokan.Yastah.Data.Users
{
    public class UserIdentityViewModel
    {
        public UserIdentityViewModel(
            ulong id,
            string username,
            string discriminator)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
        }

        public ulong Id { get; }

        public string Username { get; }

        public string Discriminator { get; }
    }
}
