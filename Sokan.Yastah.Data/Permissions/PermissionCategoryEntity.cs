using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Permissions
{
    [Table("PermissionCategories", Schema = "Permissions")]
    internal class PermissionCategoryEntity
    {
        [Key]
        public int Id { get; set; }

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
