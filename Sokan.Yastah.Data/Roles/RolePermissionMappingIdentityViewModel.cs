using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RolePermissionMappingIdentityViewModel
    {
        public RolePermissionMappingIdentityViewModel(
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

        internal static readonly Expression<Func<RolePermissionMappingEntity, RolePermissionMappingIdentityViewModel>> FromEntityProjection
            = e => new RolePermissionMappingIdentityViewModel(
                e.Id,
                e.RoleId,
                e.PermissionId);
    }
}
