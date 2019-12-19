using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Characters
{
    public class CharacterGuildUpdateModel
    {
        [Required]
        public string Name { get; set; }
            = null!;
    }
}
