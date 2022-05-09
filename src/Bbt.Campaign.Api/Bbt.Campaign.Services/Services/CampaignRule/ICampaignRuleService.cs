using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.CampaignRule
{
    public interface ICampaignRuleService
    {
        public Task<BaseResponse<CampaignRuleDto>> GetCampaignRuleAsync(int id, string userid);
        public Task<BaseResponse<CampaignRuleDto>> AddAsync(AddCampaignRuleRequest campaignRule, string userid);
        public Task<BaseResponse<CampaignRuleDto>> UpdateAsync(AddCampaignRuleRequest campaignRule, string userid);
        public Task<BaseResponse<List<CampaignRuleDto>>> GetListAsync();
        public Task<BaseResponse<CampaignRuleDto>> DeleteAsync(int id, string userid);
        public Task<BaseResponse<CampaignRuleInsertFormDto>> GetInsertForm(string userid);
        public Task<BaseResponse<CampaignRuleUpdateFormDto>> GetUpdateForm(int campaignId, string userid);
        public Task<CampaignRuleDto> GetCampaignRuleDto(int campaignId);
        public Task<BaseResponse<GetFileResponse>> GetRuleIdentityFileAsync(int campaignId);
    }
}
