using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class ConstantsEntity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(1000), Required]
        public string Name { get; set; }

        [MaxLength(100), Required]
        public string Code { get; set; }
    }
}
