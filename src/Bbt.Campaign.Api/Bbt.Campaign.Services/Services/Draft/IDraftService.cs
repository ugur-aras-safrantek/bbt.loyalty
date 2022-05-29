using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
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
            AddCampaignRuleRequest campaignRuleUpdateRequest,
            CampaignTargetInsertRequest campaignTargetInsertRequest,
            CampaignChannelCodeUpdateRequest campaignChannelCodeUpdateRequest,
            CampaignAchievementInsertRequest campaignAchievementInsertRequest
            );
        public CampaignEntity SetCampaignDefaults(CampaignEntity entity);
        public CampaignEntity SetCampaignUpdateRequest(CampaignEntity campaignEntity, CampaignUpdateRequest campaignUpdateRequest);
        public Task<int> GetProcessType(int canpaignId);
    }
}
