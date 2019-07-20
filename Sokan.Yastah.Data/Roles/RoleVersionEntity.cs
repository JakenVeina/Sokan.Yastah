using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Roles
{
    [Table("RoleVersions", Schema = "Roles")]
    internal class RoleVersionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }

        public RoleEntity Role { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Action))]
        public long ActionId { get; set; }

        public AdministrationActionEntity Action { get; set; }

        public long? PreviousVersionId { get; set; }

        public RoleVersionEntity PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public RoleVersionEntity NextVersion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<RoleVersionEntity>(entityBuilder =>
            {
                entityBuilder
                    .HasOne(x => x.PreviousVersion)
                    .WithOne()
                    .HasForeignKey<RoleVersionEntity>(x => x.PreviousVersionId);

                entityBuilder
                    .HasOne(x => x.NextVersion)
                    .WithOne()
                    .HasForeignKey<RoleVersionEntity>(x => x.NextVersionId);
            });
    }
}
