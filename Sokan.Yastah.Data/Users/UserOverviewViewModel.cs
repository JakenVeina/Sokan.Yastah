using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserOverviewViewModel
        : UserIdentityViewModel
    {
        public UserOverviewViewModel(
                ulong id,
                string username,
                string discriminator,
                DateTimeOffset firstSeen,
                DateTimeOffset lastSeen)
            : base(
                id,
                username,
                discriminator)
        {
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
        }

        public DateTimeOffset FirstSeen { get; }

        public DateTimeOffset LastSeen { get; }

        internal static readonly Expression<Func<UserEntity, UserOverviewViewModel>> FromEntityProjection
            = e => new UserOverviewViewModel(
                e.Id,
                e.Username,
                e.Discriminator,
                e.FirstSeen,
                e.LastSeen);
    }
}
