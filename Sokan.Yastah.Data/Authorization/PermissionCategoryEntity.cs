using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Authorization
{
    [Table("PermissionCategories")]
    public class PermissionCategoryEntity
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Description { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<PermissionCategoryEntity>(entityBuilder =>
            {
                entityBuilder
                    .HasIndex(x => x.Name)
                    .IsUnique();
            });
    }
}
