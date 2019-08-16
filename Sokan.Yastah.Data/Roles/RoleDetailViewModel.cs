using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RoleDetailViewModel
        : RoleIdentityViewModel
    {
        public IReadOnlyList<int> GrantedPermissionIds { get; internal set; }

        internal static Expression<Func<RoleVersionEntity, RoleDetailViewModel>> FromVersionEntityExpression
            => ve => new RoleDetailViewModel()
            {
                Id = ve.RoleId,
                Name = ve.Name,
                GrantedPermissionIds = ve.Role.PermissionMappings
                    .Where(rpme => rpme.DeletionId == null)
                    .Select(rpme => rpme.PermissionId)
                    .ToArray()
            };
    }
}
