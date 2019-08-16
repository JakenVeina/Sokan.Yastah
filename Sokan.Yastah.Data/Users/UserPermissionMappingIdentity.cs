using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserPermissionMappingIdentity
    {
        public long Id { get; internal set; }

        public ulong UserId { get; internal set; }

        public int PermissionId { get; internal set; }

        public bool isDenied { get; internal set; }

        internal static readonly Expression<Func<UserPermissionMappingEntity, UserPermissionMappingIdentity>> FromEntityProjection
            = e => new UserPermissionMappingIdentity()
            {
                Id = e.Id,
                UserId = e.UserId,
                PermissionId = e.PermissionId,
                isDenied = e.IsDenied
            };
    }
}
