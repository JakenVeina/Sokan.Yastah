using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Users
{
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
    }

    [ModelCreatingHandler]
    public class UserEntityModelCreatingHandler
        : IModelCreatingHandler<YastahDbContext>
    {
        public void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<UserEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.Id)
                    .HasConversion<long>();
            });
    }
}
