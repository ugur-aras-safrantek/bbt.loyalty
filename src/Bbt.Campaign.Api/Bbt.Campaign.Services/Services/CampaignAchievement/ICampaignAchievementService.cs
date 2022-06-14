﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignAchievement;

namespace Bbt.Campaign.Services.Services.CampaignAchievement
{
    public interface ICampaignAchievementService
    {
        public Task<BaseResponse<List<CampaignAchievementDto>>> AddAsync(CampaignAchievementInsertRequest request, UserRoleDto userRole);
        public Task<BaseResponse<List<CampaignAchievementDto>>> UpdateAsync(CampaignAchievementInsertRequest request, UserRoleDto userRole);
        public Task<BaseResponse<CampaignAchievementDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignAchievementDto>> GetCampaignAchievementAsync(int id);
        public Task<List<CampaignAchievementDto>> GetCampaignAchievementListDto(int campaignId);
        public Task<BaseResponse<CampaignAchievementInsertFormDto>> GetInsertFormAsync(int campaignId, UserRoleDto userRole);
        public Task<BaseResponse<List<CampaignAchievementDto>>> GetListAsync();
        public Task<BaseResponse<CampaignAchievementUpdateFormDto>> GetUpdateFormAsync(int campaignId, UserRoleDto userRole);
        public Task<List<CustomerAchievement>> GetCustomerAchievementsAsync(int campaignId, string customerCode, string lang);
        public Task<BaseResponse<bool>> SendToAppropval(int campaignId, UserRoleDto userRole);
    }
}
