using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class TriggerTimeEntity : AuditableEntity
    {
        [MaxLength(100), Required]
        public string Name { get; set; }
    }
}
