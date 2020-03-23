using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Auditing
{
    [Table("AuditableActionCategories", Schema = "Auditing")]
    internal class AuditableActionCategoryEntity
    {
        public AuditableActionCategoryEntity(
            int id,
            string name)
        {
            Id = id;
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; }

        [Required]
        public string Name { get; }
    }

    internal class AdministrationActionCategoryEntityTypeConfiguration
        : IEntityTypeConfiguration<AuditableActionCategoryEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionCategoryEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Id);

            entityBuilder
                .HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
