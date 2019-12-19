using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Characters
{
    public class CharacterGuildCreationModel
    {
        [Required]
        public string Name { get; set; }
            = null!;
    }
}
