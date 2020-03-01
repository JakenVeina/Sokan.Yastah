using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    }

    internal class CharacterLevelDefinitionEntityTypeConfiguration
        : IEntityTypeConfiguration<CharacterLevelDefinitionEntity>
    {
        public void Configure(
            EntityTypeBuilder<CharacterLevelDefinitionEntity> entityBuilder) { }
    }
}
