using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("CampaignTargetListView")]
    public class CampaignTargetListEntity
    {
        [Key]
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public int TargetId { get; set; }
        public int TargetGroupId { get; set; }
        public int TargetOperationId { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int TargetSourceId { get; set; }
        public int TargetViewTypeId { get; set; }
        public int? TriggerTimeId { get; set; } 
        public int? VerificationTimeId { get; set; }
        public string? FlowName { get; set; } 
        public decimal? TotalAmount { get; set; }  
        public int? NumberOfTransaction { get; set; } 
        public string? Query { get; set; }
        public string? Condition { get; set; } 
    }
}
