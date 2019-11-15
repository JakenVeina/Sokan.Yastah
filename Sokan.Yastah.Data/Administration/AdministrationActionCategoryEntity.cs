using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActionCategories", Schema = "Administration")]
    internal class AdministrationActionCategoryEntity
    {
        public AdministrationActionCategoryEntity(
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

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionCategoryEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.Id);

                entityBuilder
                    .HasIndex(x => x.Name)
                    .IsUnique();
            });
    }
}
