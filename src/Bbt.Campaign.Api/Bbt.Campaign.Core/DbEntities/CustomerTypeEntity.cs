using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CustomerTypeEntity : AuditableEntity
    {
        [MaxLength(250), Required]
        public string Name { get; set; }
        public int Code { get; set; }
    }
}
