using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserDetailViewModel
        : UserOverviewViewModel
    {
        public IReadOnlyList<int> GrantedPermissionIds { get; internal set; }

        public IReadOnlyList<int> DeniedPermissionIds { get; internal set; }

        public IReadOnlyList<long> AssignedRoleIds { get; internal set; }

        new internal static Expression<Func<UserEntity, UserDetailViewModel>> FromEntityProjection
            => e => new UserDetailViewModel()
            {
                Id = e.Id,
                Username = e.Username,
                Discriminator = e.Discriminator,
                FirstSeen = e.FirstSeen,
                LastSeen = e.LastSeen,
                GrantedPermissionIds = e.PermissionMappings
                    .Where(pm => !pm.IsDenied)
                    .Where(pm => pm.DeletionId == null)
                    .Select(pm => pm.PermissionId)
                    .ToArray(),
                DeniedPermissionIds = e.PermissionMappings
                    .Where(pm => pm.IsDenied)
                    .Where(pm => pm.DeletionId == null)
                    .Select(pm => pm.PermissionId)
                    .ToArray(),
                AssignedRoleIds = e.RoleMappings
                    .Where(rm => rm.DeletionId == null)
                    .Select(rm => rm.RoleId)
                    .ToArray()
            };
    }
}
