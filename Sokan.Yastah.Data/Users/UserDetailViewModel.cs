using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Sokan.Yastah.Data.Users
{
    public class UserDetailViewModel
        : UserOverviewViewModel
    {
        public UserDetailViewModel(
                ulong id,
                string username,
                string discriminator,
                DateTimeOffset firstSeen,
                DateTimeOffset lastSeen,
                IReadOnlyList<int> grantedPermissionIds,
                IReadOnlyList<int> deniedPermissionIds,
                IReadOnlyList<long> assignedRoleIds)
            : base(
                id,
                username,
                discriminator,
                firstSeen,
                lastSeen)
        {
            GrantedPermissionIds = grantedPermissionIds;
            DeniedPermissionIds = deniedPermissionIds;
            AssignedRoleIds = assignedRoleIds;
        }

        public IReadOnlyList<int> GrantedPermissionIds { get; }

        public IReadOnlyList<int> DeniedPermissionIds { get; }

        public IReadOnlyList<long> AssignedRoleIds { get; }

        new internal static Expression<Func<UserEntity, UserDetailViewModel>> FromEntityProjection
            = e => new UserDetailViewModel(
                e.Id,
                e.Username,
                e.Discriminator,
                e.FirstSeen,
                e.LastSeen,
                e.PermissionMappings
                    .Where(pm => !pm.IsDenied)
                    .Where(pm => pm.DeletionId == null)
                    .Select(pm => pm.PermissionId)
                    .ToArray(),
                e.PermissionMappings
                    .Where(pm => pm.IsDenied)
                    .Where(pm => pm.DeletionId == null)
                    .Select(pm => pm.PermissionId)
                    .ToArray(),
                e.RoleMappings
                    .Where(rm => rm.DeletionId == null)
                    .Select(rm => rm.RoleId)
                    .ToArray());
    }
}
