using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserPermissionMappingIdentity
    {
        public UserPermissionMappingIdentity(
            long id,
            ulong userId,
            int permissionId,
            bool isDenied)
        {
            Id = id;
            UserId = userId;
            PermissionId = permissionId;
            IsDenied = isDenied;
        }

        public long Id { get; }

        public ulong UserId { get; }

        public int PermissionId { get; }

        public bool IsDenied { get; }

        internal static readonly Expression<Func<UserPermissionMappingEntity, UserPermissionMappingIdentity>> FromEntityProjection
            = e => new UserPermissionMappingIdentity(
                e.Id,
                e.UserId,
                e.PermissionId,
                e.IsDenied);
    }
}
