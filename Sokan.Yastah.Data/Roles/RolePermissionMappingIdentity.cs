using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RolePermissionMappingIdentity
    {
        public RolePermissionMappingIdentity(
            long id,
            long roleId,
            int permissionId)
        {
            Id = id;
            RoleId = roleId;
            PermissionId = permissionId;
        }

        public long Id { get; }

        public long RoleId { get; }

        public int PermissionId { get; }

        internal static readonly Expression<Func<RolePermissionMappingEntity, RolePermissionMappingIdentity>> FromEntityProjection
            = e => new RolePermissionMappingIdentity(
                e.Id,
                e.RoleId,
                e.PermissionId);
    }
}
