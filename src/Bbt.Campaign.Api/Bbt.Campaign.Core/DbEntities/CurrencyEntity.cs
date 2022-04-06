using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CurrencyEntity : AuditableEntity
    {
        [MaxLength(100), Required]
        public string Name { get; set; }
        [MaxLength(4), Required]
        public string Code { get; set; }
    }
}
