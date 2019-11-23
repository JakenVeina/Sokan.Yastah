using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sokan.Yastah.Data.Characters
{
    internal class CharacterGuildEntity
    {
        public CharacterGuildEntity(
            long id)
        {
            Id = id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }
    }
}
