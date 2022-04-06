using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.CampaignAchievement;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignViewFormDto : CampaignParameterFormDto
    {
        public int CampaignId { get; set; }
        public CampaignDto Campaign { get; set; }
        public CampaignRuleDto CampaignRule { get; set; }
        public CampaignTargetDto CampaignTargetList { get; set; }
        public List<ChannelsAndAchievementsByCampaignDto> ChannelsAndAchievementsList { get; set; }
    }
}
