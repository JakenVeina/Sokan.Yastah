using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Authorization;

namespace Sokan.Yastah.Data
{
    public partial class YastahDbContext
    {
        internal DbSet<PermissionEntity> Permissions { get; set; }

        internal DbSet<PermissionCategoryEntity> PermissionCategories { get; set; }
    }
}
