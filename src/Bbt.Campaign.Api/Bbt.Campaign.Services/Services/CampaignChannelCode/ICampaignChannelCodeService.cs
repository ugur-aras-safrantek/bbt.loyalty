using AutoMapper;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignChannelCode;
using Bbt.Campaign.Public.Models.CampaignChannelCode;

namespace Bbt.Campaign.Services.Services.CampaignChannelCode
{
    public interface ICampaignChannelCodeService
    {
        public Task<BaseResponse<CampaignChannelCodeDto>> AddAsync(CampaignChannelCodeUpdateRequest request, UserRoleDto2 userRole);
        public Task<BaseResponse<CampaignChannelCodeDto>> UpdateAsync(CampaignChannelCodeUpdateRequest request, UserRoleDto2 userRole);

        public Task<BaseResponse<CampaignChannelCodeInsertFormDto>> GetInsertFormAsync(int campaignId, UserRoleDto2 userRole);

        public Task<BaseResponse<CampaignChannelCodeUpdateFormDto>> GetUpdateFormAsync(int campaignId, UserRoleDto2 userRole);

        public Task<List<string>> GetCampaignChannelCodeList(int campaignId);
        public Task<string> GetCampaignChannelCodesAsString(int campaignId);
    }
}
