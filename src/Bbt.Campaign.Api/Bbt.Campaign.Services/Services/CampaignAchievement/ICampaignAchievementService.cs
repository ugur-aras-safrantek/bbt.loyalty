using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignAchievement;

namespace Bbt.Campaign.Services.Services.CampaignAchievement
{
    public interface ICampaignAchievementService
    {
        public Task<BaseResponse<List<CampaignAchievementDto>>> UpdateAsync(CampaignAchievementInsertRequest request);
        public Task<BaseResponse<CampaignAchievementDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignAchievementDto>> GetCampaignAchievementAsync(int id);
        public Task<List<CampaignAchievementDto>> GetCampaignAchievementListDto(int campaignId);
        public Task<BaseResponse<CampaignAchievementInsertFormDto>> GetInsertFormAsync(int campaignId);
        public Task<BaseResponse<List<CampaignAchievementDto>>> GetListAsync();
        public Task<BaseResponse<CampaignAchievementUpdateFormDto>> GetUpdateFormAsync(int campaignId);
    }
}
