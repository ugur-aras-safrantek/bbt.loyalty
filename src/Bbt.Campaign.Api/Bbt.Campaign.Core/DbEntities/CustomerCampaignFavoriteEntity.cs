using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CustomerCampaignFavoriteEntity : AuditableEntity
    {
        [Required]
        public string CustomerCode { get; set; }

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsFavorite { get; set; }

        [MaxLength(250)]
        public string? RefId { get; set; }
        public string? ApiResponse { get; set; }
    }
}
