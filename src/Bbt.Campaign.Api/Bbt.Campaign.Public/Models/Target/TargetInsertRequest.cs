using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Public.Models.Target
{
    public class TargetInsertRequest
    {
        [MaxLength(250), Required]
        public string Name { get; set; }
        [MaxLength(250), Required]
        public string Title { get; set; }
        public bool IsActive { get; set; }
    }
}
