using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActionCategories", Schema = "Administration")]
    internal class AdministrationActionCategoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionCategoryEntity>(entityBuilder =>
            {
                entityBuilder
                    .HasIndex(x => x.Name)
                    .IsUnique();
            });
    }
}
