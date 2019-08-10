using System;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RolePermissionMappingIdentity
    {
        public long Id { get; internal set; }

        public long RoleId { get; internal set; }

        public int PermissionId { get; internal set; }

        internal static readonly Expression<Func<RolePermissionMappingEntity, RolePermissionMappingIdentity>> FromEntityProjection
            = e => new RolePermissionMappingIdentity()
            {
                Id = e.Id,
                RoleId = e.RoleId,
                PermissionId = e.PermissionId
            };
    }
}
