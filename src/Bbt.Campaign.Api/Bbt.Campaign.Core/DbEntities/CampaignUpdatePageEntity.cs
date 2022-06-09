using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignUpdatePageEntity
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }
        public bool IsCampaignUpdated { get; set; }
        public bool IsCampaignRuleUpdated { get; set; }
        public bool IsCampaignTargetUpdated { get; set; }
        public bool IsCampaignChannelCodeUpdated { get; set; }
        public bool IsCampaignAchievementUpdated { get; set; }
    }
}
