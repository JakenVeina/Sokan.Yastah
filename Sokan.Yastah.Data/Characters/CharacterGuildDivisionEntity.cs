using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterGuildDivisions", Schema = "Characters")]
    internal class CharacterGuildDivisionEntity
    {
        public CharacterGuildDivisionEntity(
            long id,
            long guildId)
        {
            Id = id;
            GuildId = guildId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }

        [ForeignKey(nameof(Guild))]
        public long GuildId { get; }

        public CharacterGuildEntity Guild { get; internal set; }
            = null!;

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterGuildDivisionEntity>();
    }
}
