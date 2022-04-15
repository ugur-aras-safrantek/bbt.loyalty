using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bbt.Campaign.Core.DbEntities
{
    public class CustomerCampaignEntity : AuditableEntity
    {
        [Required]
        public string CustomerCode { get; set; }

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public bool IsJoin { get; set; }
        public bool IsFavorite { get; set; }
    }
}
