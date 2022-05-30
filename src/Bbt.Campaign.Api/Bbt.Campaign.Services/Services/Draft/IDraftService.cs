using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;

namespace Bbt.Campaign.Services.Services.Draft
{
    public interface IDraftService
    {
        public Task<int> CreateCampaignDraftAsync(int campaignId, string userid);
        public Task<BaseResponse<CampaignDto>> CreateCampaignCopyAsync(int campaignId, string userid);
        public Task<CampaignProperty> GetCampaignProperties(int campaignId);
        public Task<int> GetProcessType(int canpaignId);
        public Task<CampaignEntity> CopyCampaignInfo(int campaignId, string userid, bool isIncludeUpdateInfo);
        public Task<CampaignRuleEntity> CopyCampaignRuleInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignTargetEntity>> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
    }
}
