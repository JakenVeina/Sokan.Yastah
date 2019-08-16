using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Users
{
    public class UserUpdateModel
    {
        [Required]
        public IReadOnlyCollection<int> GrantedPermissionIds { get; set; }

        [Required]
        public IReadOnlyCollection<int> DeniedPermissionIds { get; set; }

        [Required]
        public IReadOnlyCollection<long> AssignedRoleIds { get; set; }
    }
}
