using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Authorization
{
    internal class PermissionEntity
    {
        [Key]
        public long PermissionId { get; set; }

        [ForeignKey(nameof(Category))]
        public long CategoryId { get; set; }

        public PermissionCategoryEntity Category { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Description { get; set; }
    }

    [ModelCreatingHandler]
    public class PermissionEntityModelCreatingHandler
        : IModelCreatingHandler<YastahDbContext>
    {
        public void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<PermissionEntity>(entityBuilder =>
            {
                entityBuilder
                    .HasIndex(x => new { x.CategoryId, x.Name })
                    .IsUnique();
            });
    }
}
