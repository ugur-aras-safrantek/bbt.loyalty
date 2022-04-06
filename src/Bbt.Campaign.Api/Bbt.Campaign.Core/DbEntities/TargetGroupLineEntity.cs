using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class TargetGroupLineEntity : AuditableEntity
    {
        [ForeignKey("TargetGroups")]
        public int TargetGroupId { get; set; }
        public TargetGroupEntity TargetGroup { get; set; }

        [ForeignKey("Target")]
        public int TargetId { get; set; }
        public TargetEntity Target { get; set; }
    }
}
