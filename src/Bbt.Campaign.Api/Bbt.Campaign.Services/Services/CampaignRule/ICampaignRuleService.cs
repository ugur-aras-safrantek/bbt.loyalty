﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.CampaignRule
{
    public interface ICampaignRuleService
    {
        public Task<BaseResponse<CampaignRuleDto>> GetCampaignRuleAsync(int id);
        public Task<BaseResponse<CampaignRuleDto>> AddAsync(AddCampaignRuleRequest campaignRule, string userId);
        public Task<BaseResponse<CampaignRuleDto>> UpdateAsync(AddCampaignRuleRequest campaignRule, string userId);
        public Task<BaseResponse<List<CampaignRuleDto>>> GetListAsync();
        public Task<BaseResponse<CampaignRuleDto>> DeleteAsync(int id, string userId);
        public Task<BaseResponse<CampaignRuleInsertFormDto>> GetInsertForm(string userId);
        public Task<BaseResponse<CampaignRuleUpdateFormDto>> GetUpdateForm(int campaignId, string userId);
        public Task<CampaignRuleDto> GetCampaignRuleDto(int campaignId);
        public Task<BaseResponse<GetFileResponse>> GetRuleIdentityFileAsync(int campaignId);
    }
}
