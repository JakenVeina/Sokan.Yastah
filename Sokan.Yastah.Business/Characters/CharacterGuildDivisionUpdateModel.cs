using System.ComponentModel.DataAnnotations;

namespace Sokan.Yastah.Business.Characters
{
    public class CharacterGuildDivisionUpdateModel
    {
        [Required]
        public string Name { get; set; }
            = null!;
    }
}
