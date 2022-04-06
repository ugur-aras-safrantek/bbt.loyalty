using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignDetailEntity : AuditableEntity
    {
        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public string? CampaignListImageUrl { get; set; }
        public string? CampaignDetailImageUrl { get; set; }
        [Column(TypeName = "ntext")]
        public string? SummaryTr { get; set; }
        [Column(TypeName = "ntext")]
        public string? SummaryEn { get; set; }
        [Column(TypeName = "ntext")]
        public string? ContentTr { get; set; }
        [Column(TypeName = "ntext")]
        public string? ContentEn { get; set; }
        [Column(TypeName = "ntext")]
        public string? DetailTr { get; set; }
        [Column(TypeName = "ntext")]
        public string? DetailEn { get; set; }
    }
}
