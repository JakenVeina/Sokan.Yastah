using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserRoleMappingIdentityViewModel
    {
        public UserRoleMappingIdentityViewModel(
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

        internal static readonly Expression<Func<UserRoleMappingEntity, UserRoleMappingIdentityViewModel>> FromEntityProjection
            = e => new UserRoleMappingIdentityViewModel(
                e.Id,
                e.UserId,
                e.RoleId);
    }
}
