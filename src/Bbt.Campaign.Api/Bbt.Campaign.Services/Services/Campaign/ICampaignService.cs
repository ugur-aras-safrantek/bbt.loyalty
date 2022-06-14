using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.Campaign
{
    public interface ICampaignService
    {
        public Task<BaseResponse<CampaignDto>> GetCampaignAsync(int id);
        public Task<BaseResponse<CampaignDto>> AddAsync(CampaignInsertRequest campaign, UserRoleDto userRole);
        public Task<BaseResponse<CampaignDto>> UpdateAsync(CampaignUpdateRequest campaign, UserRoleDto userRole);
        public Task<BaseResponse<CampaignDto>> CreateDraftAsync(int id, UserRoleDto userRole);
        public Task<BaseResponse<List<CampaignDto>>> GetListAsync(UserRoleDto userRole);
        public Task<BaseResponse<List<ParameterDto>>> GetParameterListAsync();
        public Task<BaseResponse<CampaignDto>> DeleteAsync(int id, UserRoleDto userRole);
        public Task<BaseResponse<CampaignInsertFormDto>> GetInsertFormAsync(UserRoleDto userRole);
        public Task<BaseResponse<CampaignUpdateFormDto>> GetUpdateFormAsync(int id, string contentRootPath, UserRoleDto userRole);
        public Task<BaseResponse<CampaignListFilterResponse>> GetByFilterAsync(CampaignListFilterRequest request, UserRoleDto userRole);
        public Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignListFilterRequest request, UserRoleDto userRole);
        public Task<CampaignDto> GetCampaignDtoAsync(int id);
        public Task<BaseResponse<GetFileResponse>> GetContractFileAsync(int id, string contentRootPath);
        public Task<GetFileResponse> GetContractFile(int id, string contentRootPath);
    }
}
