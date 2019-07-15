namespace Sokan.Yastah.Data.Users
{
    public class UserCreatedNotification
    {
        public UserCreatedNotification(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash)
        {
            UserId = userId;
            Username = username;
            Discriminator = discriminator;
            AvatarHash = avatarHash;
        }

        public ulong UserId { get; }

        public string Username { get; }

        public string Discriminator { get; }

        public string AvatarHash { get; }
    }
}
