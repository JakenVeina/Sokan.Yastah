using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Auditing
{
    [Table("AuditableActionTypes", Schema = "Auditing")]
    internal class AuditableActionTypeEntity
    {
        public AuditableActionTypeEntity(
            int id,
            int categoryId,
            string name)
        {
            Id = id;
            CategoryId = categoryId;
            Name = name;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; }

        public AuditableActionCategoryEntity Category { get; internal set; }
            = null!;

        [Required]
        public string Name { get; }
    }

    internal class AdministrationActionTypeEntityTypeConfiguration
        : IEntityTypeConfiguration<AuditableActionTypeEntity>
    {
        public void Configure(
            EntityTypeBuilder<AuditableActionTypeEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Id);

            entityBuilder
                .HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
