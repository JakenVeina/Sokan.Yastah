using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sokan.Yastah.Data.Users
{
    [Table("Users", Schema = "Users")]
    internal class UserEntity
    {
        public UserEntity(
            ulong id,
            string username,
            string discriminator,
            string? avatarHash,
            DateTimeOffset firstSeen,
            DateTimeOffset lastSeen)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            AvatarHash = avatarHash;
            FirstSeen = firstSeen;
            LastSeen = lastSeen;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public ulong Id { get; internal set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Discriminator { get; set; }

        public string? AvatarHash { get; set; }

        public DateTimeOffset FirstSeen { get; }

        public DateTimeOffset LastSeen { get; set; }

        public ICollection<UserPermissionMappingEntity> PermissionMappings { get; internal set; }
            = null!;

        public ICollection<UserRoleMappingEntity> RoleMappings { get; internal set; }
            = null!;
    }

    internal class UserEntityTypeConfiguration
        : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(
            EntityTypeBuilder<UserEntity> entityBuilder)
        {
            entityBuilder
                .Property(x => x.Id)
                .HasConversion<long>();

            entityBuilder
                .Property(x => x.FirstSeen);
        }
    }
}
