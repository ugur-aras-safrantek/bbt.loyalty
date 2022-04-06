using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Public.Dtos
{
    public class ViewOptionDto
    {
        [MaxLength(250), Required]
        public string Name { get; set; }

        [MaxLength(4), Required]
        public string Code { get; set; }
    }
}
