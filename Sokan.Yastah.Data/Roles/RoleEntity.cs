using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Roles
{
    [Table("Roles", Schema = "Roles")]
    internal class RoleEntity
    {
        public RoleEntity(
            long id)
        {
            Id = id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        public ICollection<RolePermissionMappingEntity> PermissionMappings { get; internal set; }
            = null!;

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<RoleEntity>();
    }
}
