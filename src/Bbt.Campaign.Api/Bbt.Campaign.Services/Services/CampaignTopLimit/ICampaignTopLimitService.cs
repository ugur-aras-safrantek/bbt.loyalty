using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Models.CampaignTopLimit;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.CampaignTopLimit
{
    public interface ICampaignTopLimitService
    {
        public Task<BaseResponse<TopLimitDto>> GetCampaignTopLimitAsync(int id);
        public Task<BaseResponse<TopLimitDto>> AddAsync(CampaignTopLimitInsertRequest campaignTopLimit, UserRoleDto userRole);
        public Task<BaseResponse<TopLimitDto>> UpdateAsync(CampaignTopLimitUpdateRequest campaignTopLimit, UserRoleDto userRole);
        public Task<BaseResponse<List<TopLimitDto>>> GetListAsync();
        public Task<BaseResponse<TopLimitDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignTopLimitInsertFormDto>> GetInsertForm(UserRoleDto userRole);
        public Task<BaseResponse<CampaignTopLimitFilterParameterResponse>> GetFilterParameterList();
        public Task<BaseResponse<CampaignTopLimitUpdateFormDto>> GetUpdateForm(int id, UserRoleDto userRole);
        public Task<BaseResponse<CampaignTopLimitListFilterResponse>> GetByFilterAsync(CampaignTopLimitListFilterRequest request, UserRoleDto userRole);
        public Task<BaseResponse<GetFileResponse>> GetExcelAsync(CampaignTopLimitListFilterRequest request, UserRoleDto userRole);
        public Task<bool> IsActiveTopLimit(int id);
        public Task<TopLimitDto> GetTopLimitDto(int id);
    }
}
