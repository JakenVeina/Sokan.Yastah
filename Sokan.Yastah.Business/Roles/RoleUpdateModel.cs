using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Roles
{
    public class RoleUpdateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IReadOnlyCollection<int> GrantedPermissionIds { get; set; }
    }
}
