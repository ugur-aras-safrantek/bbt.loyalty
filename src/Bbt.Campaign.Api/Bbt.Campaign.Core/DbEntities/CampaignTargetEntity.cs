using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignTargetEntity :AuditableEntity
    {

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }

        [ForeignKey("TargetOperation")]
        public int TargetOperationId { get; set; }
        public TargetOperationEntity TargetOperation { get; set; }

        [ForeignKey("Targets")]
        public int TargetId { get; set; }
        public TargetEntity Target { get; set; }

        [ForeignKey("TargetGroups")]
        public int TargetGroupId { get; set; }
        public TargetGroupEntity TargetGroup { get; set; }
    }
}
