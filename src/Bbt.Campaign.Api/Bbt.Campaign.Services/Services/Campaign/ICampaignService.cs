using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public interface ICampaignService
    {
        public Task<BaseResponse<CampaignDto>> GetCampaignAsync(int id, string userid);
        public Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign, string userid);
        public Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign, string userid);
        public Task<BaseResponse<CampaignDto>> CreateDraftAsync(int id, string userid);
        public Task<BaseResponse<List<CampaignDto>>> GetListAsync(string userid);
        public Task<BaseResponse<List<ParameterDto>>> GetParameterListAsync();
        public Task<BaseResponse<CampaignDto>> DeleteAsync(int id, string userid);
        public Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync(string userid);
        public Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath, string userid);
        public Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request, string userid);
        public Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request, string userid);
        public Task<CampaignDto> GetCampaignDtoAsync(int id);
        public Task<BaseResponse<GetFileResponse>> GetContractFileAsync(int id, string contentRootPath);
        public Task<GetFileResponse> GetContractFile(int id, string contentRootPath);
        //public Task<bool> IsInvisibleCampaign(int campaignId);
        public Task<bool> IsActiveCampaign(int campaignId);
    }
}
