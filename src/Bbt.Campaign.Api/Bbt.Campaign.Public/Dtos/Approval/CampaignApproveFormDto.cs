using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Dtos.CampaignTarget;

using Bbt.Campaign.Public.Models.CampaignAchievement;


namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignApproveFormDto : CampaignParameterFormDto
    {
        public bool isNewRecord { get; set; }
        public CampaignDto CampaignDraft { get; set; }
        public CampaignRuleDto CampaignRuleDraft { get; set; }
        public CampaignAchievementDto CampaignAchievementDraft { get; set; }
        public List<CampaignTargetDto> CampaignTargetListDraft { get; set; }

        public ChannelsAndAchievementsByCampaignResponse AchievementsListDraft;

        public CampaignDto Campaign { get; set; }
        public CampaignDetailDto CampaignDetail { get; set; }
        public CampaignRuleDto CampaignRule { get; set; }
        public CampaignAchievementDto CampaignAchievement { get; set; }
        public List<CampaignTargetDto> CampaignTargetList { get; set; }

        public ChannelsAndAchievementsByCampaignResponse AchievementsList;
    }
}
