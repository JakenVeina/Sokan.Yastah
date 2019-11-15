using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Roles
{
    public class RoleCreationModel
    {
        [Required]
        public string Name { get; set; }
            = null!;

        [Required]
        public IReadOnlyCollection<int> GrantedPermissionIds { get; set; }
            = null!;
    }
}
