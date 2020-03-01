using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Permissions
{
    [Table("Permissions", Schema = "Permissions")]
    internal class PermissionEntity
    {
        public PermissionEntity(
            int permissionId,
            int categoryId,
            string name,
            string description)
        {
            PermissionId = permissionId;
            CategoryId = categoryId;
            Name = name;
            Description = description;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PermissionId { get; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; }

        public PermissionCategoryEntity Category { get; internal set; }
            = null!;

        [Required]
        public string Name { get; }

        [Required]
        public string Description { get; }
    }

    internal class PermissionEntityTypeConfiguration
        : IEntityTypeConfiguration<PermissionEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.PermissionId);

            entityBuilder
                .HasIndex(x => new { x.CategoryId, x.Name })
                .IsUnique();

            entityBuilder
                .Property(x => x.Description);
        }
    }
}
