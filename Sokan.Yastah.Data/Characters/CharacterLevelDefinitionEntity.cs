using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Sokan.Yastah.Data.Characters
{
    [Table("CharacterLevelDefinitions", Schema = "Characters")]
    internal class CharacterLevelDefinitionEntity
    {
        public CharacterLevelDefinitionEntity(
            int level)
        {
            Level = level;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Level { get; internal set; }

        [OnModelCreating]
        public static void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<CharacterLevelDefinitionEntity>();
    }
}
