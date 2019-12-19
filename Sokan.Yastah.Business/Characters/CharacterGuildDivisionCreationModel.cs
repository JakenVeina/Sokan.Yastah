using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Characters
{
    public class CharacterGuildDivisionCreationModel
    {
        [Required]
        public string Name { get; set; }
            = null!;
    }
}
