using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;

namespace Bbt.Campaign.Services.Services.Draft
{
    public interface IDraftService
    {
        public Task<int> CreateCampaignDraftAsync(int campaignId, string userid, int pageTypeId);
        public Task<BaseResponse<CampaignDto>> CreateCampaignCopyAsync(int campaignId, string userid);
        public Task<CampaignProperty> GetCampaignProperties(int campaignId);
        public Task<int> GetCampaignProcessType(int campaignId);
        public Task<CampaignEntity> CopyCampaignInfo(CampaignEntity targetEntity, int sourceCampaignId, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo, 
            bool isIncludeOrder, bool isIncludeCode, bool isIncludeStatusId);
        public Task<List<CampaignDocumentEntity>> CopyCampaignDocumentInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeUpdateInfo);
        public Task<List<CampaignTargetEntity>> CopyCampaignTargetInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignChannelCodeEntity>> CopyCampaignChannelCodeInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignAchievementEntity>> CopyCampaignAchievementInfo(int campaignId, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeCode);

        public Task<CampaignRuleEntity> CopyCampaignRuleInfo(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, CampaignEntity campaignEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleBranchEntity>> CopyCampaignRuleBranches(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleBusinessLineEntity>> CopyCampaignRuleBusiness(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleCustomerTypeEntity>> CopyCampaignRuleCustomerTypes(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);
        public Task<List<CampaignRuleIdentityEntity>> CopyCampaignRuleIdentites(CampaignRuleEntity campaignRuleDraftEntity, CampaignRuleEntity campaignRuleEntity, string userid, bool isIncludeCreateInfo, bool isIncludeUpdateInfo);

        public Task<int> GetTopLimitProcessType(int topLimitId);

        public Task<TopLimitEntity> CopyTopLimitInfo(int topLimitId, TopLimitEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo,
            bool isIncludeCode, bool isIncludeStatusId);
        public Task<List<CampaignTopLimitEntity>> CopyCampaignTopLimits(int topLimitId, TopLimitEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo);

        public Task<int> GetTargetProcessType(int targetId);
        public Task<TargetEntity> CopyTargetInfo(int targetId, TargetEntity targetEntity, string userid,
            bool isIncludeCreateInfo, bool isIncludeUpdateInfo, bool isIncludeApproveInfo,
            bool isIncludeCode, bool isIncludeStatusId);

        public Task<bool> IsActiveCampaign(int campaignId);
    }
}
