using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignTarget;

namespace Bbt.Campaign.Services.Services.Draft
{
    public interface IDraftService
    {
        public Task<int> CreateCampaignDraft(
            int campaignId,
            int campaignPageId,
            string userid,
            CampaignUpdateRequest campaignUpdateRequest,
            AddCampaignRuleRequest addCampaignRuleRequest,
            CampaignTargetInsertRequest campaignTargetInsertRequest,
            CampaignAchievementInsertRequest campaignAchievementInsertRequest
            );
    }
}
