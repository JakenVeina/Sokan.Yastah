using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data
{
    public partial class YastahDbContext
    {
        internal DbSet<RoleEntity> Roles { get; set; }

        internal DbSet<RolePermissionMappingEntity> RolePermissionMappings { get; set; }
    }
}
