using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserOverviewViewModel
        : UserIdentityViewModel
    {
        public DateTimeOffset FirstSeen { get; internal set; }

        public DateTimeOffset LastSeen { get; internal set; }

        internal  static readonly Expression<Func<UserEntity, UserOverviewViewModel>> FromEntityProjection
            = e => new UserOverviewViewModel()
            {
                Id = e.Id,
                Username = e.Username,
                Discriminator = e.Discriminator,
                FirstSeen = e.FirstSeen,
                LastSeen = e.LastSeen
            };
    }
}
