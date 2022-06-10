using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Core.BaseEntities
{
    public class AuditableEntity : IAuditableBaseEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        [MaxLength(100)]
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
