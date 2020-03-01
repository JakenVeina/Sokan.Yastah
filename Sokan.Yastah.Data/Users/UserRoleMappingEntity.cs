using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Sokan.Yastah.Data.Administration;
using Sokan.Yastah.Data.Roles;

namespace Sokan.Yastah.Data.Users
{
    [Table("UserRoleMappings", Schema = "Users")]
    internal class UserRoleMappingEntity
    {
        public UserRoleMappingEntity(
            long id,
            ulong userId,
            long roleId,
            long creationId,
            long? deletionId)
        {
            Id = id;
            UserId = userId;
            RoleId = roleId;
            CreationId = creationId;
            DeletionId = deletionId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(User))]
        public ulong UserId { get; }

        public UserEntity User { get; internal set; }
            = null!;

        [ForeignKey(nameof(Role))]
        public long RoleId { get; }

        public RoleEntity Role { get; internal set; }
            = null!;

        [ForeignKey(nameof(Creation))]
        public long CreationId { get; }

        public AdministrationActionEntity Creation { get; internal set; }
            = null!;

        [ForeignKey(nameof(Deletion))]
        public long? DeletionId { get; set; }

        public AdministrationActionEntity? Deletion { get; set; }
    }

    internal class UserRoleMappingEntityTypeConfiguration
        : IEntityTypeConfiguration<UserRoleMappingEntity>
    {
        public void Configure(
            EntityTypeBuilder<UserRoleMappingEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.UserId)
                .HasConversion<long>();
        }
    }
}
