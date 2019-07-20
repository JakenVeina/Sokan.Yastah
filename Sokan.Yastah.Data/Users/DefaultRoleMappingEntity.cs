using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Users
{
    [Table("DefaultRoleMappings", Schema = "Users")]
    internal class DefaultRoleMappingEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey(nameof(Role))]
        public long RoleId { get; set; }

        public RoleEntity Role { get; set; }

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; set; }

        public AdministrationActionEntity Creation { get; set; }

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AdministrationActionEntity Deletion { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<DefaultRoleMappingEntity>();
    }
}
