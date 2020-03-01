using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Permissions
{
    [Table("PermissionCategories", Schema = "Permissions")]
    internal class PermissionCategoryEntity
    {
        public PermissionCategoryEntity(
            int id,
            string name,
            string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; }

        [Required]
        public string Name { get; }

        [Required]
        public string Description { get; }

        public ICollection<PermissionEntity> Permissions { get; internal set; }
            = null!;
    }

    internal class PermissionCategoryEntityTypeConifugration
        : IEntityTypeConfiguration<PermissionCategoryEntity>
    {
        public void Configure(
            EntityTypeBuilder<PermissionCategoryEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Id);

            entityBuilder
                .HasIndex(x => x.Name)
                .IsUnique();

            entityBuilder
                .Property(x => x.Description);
        }
    }
}
