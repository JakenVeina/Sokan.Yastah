using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Sokan.Yastah.Data.Users;

namespace Sokan.Yastah.Data.Characters
{
    [Table("Characters", Schema = "Characters")]
    internal class CharacterEntity
    {
        public CharacterEntity(
            long id,
            ulong ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Owner))]
        public ulong OwnerId { get; }

        public UserEntity Owner { get; internal set; }
            = null!;

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterEntity>(entityBuilder =>
            {
                entityBuilder
                    .Property(x => x.OwnerId)
                    .HasConversion<long>();
            });
    }
}
