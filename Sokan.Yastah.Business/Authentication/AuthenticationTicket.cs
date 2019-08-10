using System.Collections.Generic;

namespace Sokan.Yastah.Business.Authentication
{
    public class AuthenticationTicket
    {
        public AuthenticationTicket(
            ulong userId,
            string username,
            string discriminator,
            string avatarHash,
            IReadOnlyDictionary<int, string> grantedPermissions)
        {
            UserId = userId;
            Username = username;
            Discriminator = discriminator;
            AvatarHash = avatarHash;
            GrantedPermissions = grantedPermissions;
        }

        public ulong UserId { get; }

        public string Username { get; }

        public string Discriminator { get; }

        public string AvatarHash { get; }

        public IReadOnlyDictionary<int, string> GrantedPermissions { get; }
    }
}
