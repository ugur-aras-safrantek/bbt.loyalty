using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.CampaignRule
{
    public interface ICampaignRuleService
    {
        public Task<BaseResponse<CampaignRuleDto>> GetCampaignRuleAsync(int id);
        public Task<BaseResponse<CampaignRuleDto>> AddAsync(AddCampaignRuleRequest campaignRule, UserRoleDto2 userRole);
        public Task<BaseResponse<CampaignRuleDto>> UpdateAsync(AddCampaignRuleRequest campaignRule, UserRoleDto2 userRole);
        public Task<BaseResponse<List<CampaignRuleDto>>> GetListAsync();
        public Task<BaseResponse<CampaignRuleDto>> DeleteAsync(int id, UserRoleDto2 userRole);
        public Task<BaseResponse<CampaignRuleInsertFormDto>> GetInsertForm(UserRoleDto2 userRole);
        public Task<BaseResponse<CampaignRuleUpdateFormDto>> GetUpdateForm(int campaignId, UserRoleDto2 userRole);
        public Task<CampaignRuleDto> GetCampaignRuleDto(int campaignId);
        public Task<BaseResponse<GetFileResponse>> GetRuleIdentityFileAsync(int campaignId);
    }
}
