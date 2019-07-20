using System;

namespace Sokan.Yastah.Data.Users
{
    public struct UserCreatedNotification
    {
        internal static UserCreatedNotification FromEntity(UserEntity entity)
            => new UserCreatedNotification(
                entity.Id,
                entity.Username,
                entity.Discriminator,
                entity.AvatarHash,
                entity.FirstSeen,
                entity.LastSeen);

        public UserCreatedNotification(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen)
        {
            UserId = userId;
            Username = username;
            Discriminator = discriminator;
            AvatarHash = avatarHash;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
        }

        public ulong UserId { get; }

        public string Username { get; }

        public string Discriminator { get; }

        public string AvatarHash { get; }

        public DateTimeOffset FirstSeen { get; }

        public DateTimeOffset LastSeen { get; }
    }
}
