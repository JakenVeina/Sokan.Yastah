using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserRoleMappingIdentity
    {
        public UserRoleMappingIdentity(
            long id,
            ulong userId,
            long roleId)
        {
            Id = id;
            UserId = userId;
            RoleId = roleId;
        }

        public long Id { get; internal set; }

        public ulong UserId { get; internal set; }

        public long RoleId { get; internal set; }

        internal static readonly Expression<Func<UserRoleMappingEntity, UserRoleMappingIdentity>> FromEntityProjection
            = e => new UserRoleMappingIdentity(
                e.Id,
                e.UserId,
                e.RoleId);
    }
}
