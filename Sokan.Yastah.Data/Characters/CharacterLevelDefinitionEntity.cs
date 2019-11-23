using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sokan.Yastah.Data.Characters
{
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
}
