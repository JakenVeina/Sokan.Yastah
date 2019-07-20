using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Administration
{
    [Table("AdministrationActionTypes", Schema = "Administration")]
    internal class AdministrationActionTypeEntity
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public AdministrationActionCategoryEntity Category { get; set; }

        [Required]
        public string Name { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<AdministrationActionTypeEntity>(entityBuilder =>
            {
                entityBuilder
                    .HasIndex(x => x.Name)
                    .IsUnique();
            });
    }
}
