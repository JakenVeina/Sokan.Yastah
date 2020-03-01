using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    }

    internal class RoleEntityTypeConfiguration
        : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(
            EntityTypeBuilder<RoleEntity> entityBuilder) { }
    }
}
