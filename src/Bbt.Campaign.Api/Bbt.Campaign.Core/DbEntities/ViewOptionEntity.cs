using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class ViewOptionEntity : AuditableEntity
    {
        [MaxLength(250), Required]
        public string Name { get; set; }

        [MaxLength(4), Required]
        public string Code { get; set; }
    }
}
