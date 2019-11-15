using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActionTypes", Schema = "Administration")]
    internal class AdministrationActionTypeEntity
    {
        public AdministrationActionTypeEntity(
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

        public AdministrationActionCategoryEntity Category { get; internal set; }
            = null!;

        [Required]
        public string Name { get; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionTypeEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.Id);

                entityBuilder
                    .HasIndex(x => x.Name)
                    .IsUnique();
            });
    }
}
