using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Roles
{
    public class RoleDetailViewModel
        : RoleIdentityViewModel
    {
        public RoleDetailViewModel(
                long id,
                string name,
                IReadOnlyList<int> grantedPermissionIds)
            : base(
                id,
                name)
        {
            GrantedPermissionIds = grantedPermissionIds;
        }

        public IReadOnlyList<int> GrantedPermissionIds { get; }

        internal static Expression<Func<RoleVersionEntity, RoleDetailViewModel>> FromVersionEntityExpression
            = ve => new RoleDetailViewModel(
                ve.RoleId,
                ve.Name,
                ve.Role.PermissionMappings
                    .Where(rpme => rpme.DeletionId == null)
                    .Select(rpme => rpme.PermissionId)
                    .ToArray());
    }
}
