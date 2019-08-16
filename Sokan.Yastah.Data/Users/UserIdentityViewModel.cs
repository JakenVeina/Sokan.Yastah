namespace Sokan.Yastah.Data.Users
{
    public class UserIdentityViewModel
    {
        public ulong Id { get; internal set; }

        public string Username { get; internal set; }

        public string Discriminator { get; internal set; }
    }
}
