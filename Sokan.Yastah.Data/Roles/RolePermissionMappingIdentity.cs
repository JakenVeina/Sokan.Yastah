using System;
using System.Linq.Expressions;

using Sokan.Yastah.Data.Authorization;

namespace Sokan.Yastah.Data.Roles
{
    public class RolePermissionMappingIdentity
    {
        public long Id { get; internal set; }

        public long RoleId { get; internal set; }

        public PermissionIdentity Permission { get; internal set; }

        internal static readonly Expression<Func<RolePermissionMappingEntity, RolePermissionMappingIdentity>> FromEntityProjection
            = entity => new RolePermissionMappingIdentity()
            {
                Id = entity.Id,
                RoleId = entity.RoleId,
                Permission = new PermissionIdentity()
                {
                    CategoryId = entity.Permission.Category.Id,
                    PermissionId = entity.Permission.PermissionId,
                    CategoryName = entity.Permission.Category.Name,
                    PermissionName = entity.Permission.Name
                }
            };
    }
}
