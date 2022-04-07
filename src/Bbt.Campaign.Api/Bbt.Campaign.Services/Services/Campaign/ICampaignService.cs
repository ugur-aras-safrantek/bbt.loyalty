using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public interface ICampaignService
    {
        public Task<BaseResponse<CampaignDto>> GetCampaignAsync(int id);
        public Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign);
        public Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign);
        public Task<BaseResponse<List<CampaignDto>>> GetListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetParameterListAsync();
        public Task<BaseResponse<CampaignDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync();
        public Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath);
        public Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request);
        public Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request);
        public Task<CampaignDto> GetCampaignDtoAsync(int id);
        public Task<BaseResponse<GetFileResponse>> GetContractFileAsync(int id, string contentRootPath);
        public Task<bool> IsInvisibleCampaign(int campaignId);
    }
}
