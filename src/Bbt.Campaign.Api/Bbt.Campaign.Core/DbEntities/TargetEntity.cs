using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class TargetEntity : AuditableEntity
    {
        [MaxLength(100), Required]
        public string Code { get; set; }

        [MaxLength(250), Required]
        public string Name { get; set; }
        [MaxLength(250), Required]
        public string Title { get; set; }
        public virtual TargetDetailEntity TargetDetail { get; set; }
        public bool IsActive { get; set; }

        //Approve
        [MaxLength(100)]
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [ForeignKey("Statuses")]
        public int StatusId { get; set; }
        public StatusEntity Status { get; set; }

    }
}
