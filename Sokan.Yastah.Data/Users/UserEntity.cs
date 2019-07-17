using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Users
{
    [Table("Users")]
    internal class UserEntity
    {
        [Key]
        public ulong Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Discriminator { get; set; }

        public string AvatarHash { get; set; }

        public DateTimeOffset FirstSeen { get; set; }

        public DateTimeOffset LastSeen { get; set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.Id)
                    .HasConversion<long>();
            });
    }
}
