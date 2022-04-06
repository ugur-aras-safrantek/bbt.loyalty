using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignAchievementChannelCodeEntity : AuditableEntity
    {

        [ForeignKey("Achievement")]
        public int AchievementId { get; set; }
        public CampaignAchievementEntity Achievement { get; set; }

        [MaxLength(100), Required]
        public string ChannelCode { get; set; }
    }
}
