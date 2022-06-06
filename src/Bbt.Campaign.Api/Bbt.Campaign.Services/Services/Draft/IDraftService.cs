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
        public Task<CampaignEntity> CopyCampaignInfo(CampaignEntity campaignEntity, int campaignId, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<CampaignRuleEntity> CopyCampaignRuleInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignTargetEntity>> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);




        public Task<CampaignRuleEntity> CopyCampaignRuleInfo2(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleBranchEntity>> CopyCampaignRuleBranches(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleBusinessLineEntity>> CopyCampaignRuleBusiness(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleCustomerTypeEntity>> CopyCampaignRuleCustomerTypes(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)
        public Task<List<CampaignRuleIdentityEntity>> CopyCampaignRuleIdentites(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo)

    }
}
