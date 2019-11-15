using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Sokan.Yastah.Data.Administration;

namespace Sokan.Yastah.Data.Roles
{
    [Table("RoleVersions", Schema = "Roles")]
    internal class RoleVersionEntity
    {
        public RoleVersionEntity(
            long id,
            long roleId,
            string name,
            bool isDeleted,
            long actionId,
            long? previousVersionId,
            long? nextVersionId)
        {
            Id = id;
            RoleId = roleId;
            Name = name;
            IsDeleted = isDeleted;
            ActionId = actionId;
            PreviousVersionId = previousVersionId;
            NextVersionId = nextVersionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; }

        public RoleEntity Role { get; internal set; }
            = null!;

        [Required]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(Action))]
        public long ActionId { get; }

        public AdministrationActionEntity Action { get; internal set; }
            = null!;

        public long? PreviousVersionId { get; set; }

        public RoleVersionEntity? PreviousVersion { get; set; }

        public long? NextVersionId { get; set; }

        public RoleVersionEntity? NextVersion { get; set; }

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
