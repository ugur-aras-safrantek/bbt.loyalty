using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Models.CampaignTarget;

namespace Bbt.Campaign.Services.Services.CampaignTarget
{
    public interface ICampaignTargetService
    {
        public Task<BaseResponse<CampaignTargetDto>> GetCampaignTargetAsync(int id);
        public Task<BaseResponse<CampaignTargetDto>> AddAsync(CampaignTargetInsertRequest request, string userId);
        public Task<BaseResponse<CampaignTargetDto>> UpdateAsync(CampaignTargetInsertRequest request, string userId);
        public Task<BaseResponse<List<CampaignTargetDto>>> GetListAsync(); 
        public Task<BaseResponse<CampaignTargetDto>> GetListByCampaignAsync(int campaignId);
        public Task<BaseResponse<CampaignTargetDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignTargetInsertFormDto>> GetInsertForm(string userId);
        public Task<BaseResponse<CampaignTargetUpdateFormDto>> GetUpdateForm(int campaignId, string userId);
        public Task<CampaignTargetDto> GetCampaignTargetDto(int campaignId, bool isRemoveInvisible);
        public Task<CampaignTargetDto> GetCampaignTargetDtoCustomer(int campaignId, decimal usedAmount, int usedNumberOfTransaction);
        public Task<CampaignTargetDto2> GetCampaignTargetDtoCustomer2(int campaignId, string customerCode, string lang, bool isTest);
    }
}
