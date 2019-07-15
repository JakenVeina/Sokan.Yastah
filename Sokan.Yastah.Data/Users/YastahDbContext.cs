using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data
{
    public partial class YastahDbContext
    {
        internal DbSet<UserEntity> Users { get; set; }

        internal DbSet<UserPermissionDefaultMappingEntity> UserPermissionDefaultMappings { get; set; }

        internal DbSet<UserPermissionMappingEntity> UserPermissionMappings { get; set; }

        internal DbSet<UserRoleDefaultMappingEntity> UserRoleDefaultMappings { get; set; }

        internal DbSet<UserRoleMappingEntity> UserRoleMappings { get; set; }
    }
}
