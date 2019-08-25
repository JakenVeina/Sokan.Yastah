using System.Collections.Generic;

namespace Sokan.Yastah.Business.Authentication
{
    public class AuthenticationTicket
    {
        public AuthenticationTicket(
            long id,
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            Id = id;
            UserId = userId;
            Username = username;
            Discriminator = discriminator;
            AvatarHash = avatarHash;
            GrantedPermissions = grantedPermissions;
        }

        public long Id { get; }

        public ulong UserId { get; }

        public string Username { get; }

        public string Discriminator { get; }

        public string AvatarHash { get; }

        public IReadOnlyDictionary<int, string> GrantedPermissions { get; }
    }
}
