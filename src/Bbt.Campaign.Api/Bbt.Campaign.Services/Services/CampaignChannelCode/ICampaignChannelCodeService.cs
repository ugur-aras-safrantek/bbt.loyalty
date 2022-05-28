using AutoMapper;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignChannelCode;
using Bbt.Campaign.Public.Models.CampaignChannelCode;

namespace Bbt.Campaign.Services.Services.CampaignChannelCode
{
    public interface ICampaignChannelCodeService
    {
        public Task<BaseResponse<CampaignChannelCodeDto>> AddAsync(CampaignChannelCodeUpdateRequest request, string userid);
        public Task<BaseResponse<CampaignChannelCodeDto>> UpdateAsync(CampaignChannelCodeUpdateRequest request, string userid);

        public Task<BaseResponse<CampaignChannelCodeInsertFormDto>> GetInsertFormAsync(int campaignId, string userid);

        public Task<BaseResponse<CampaignChannelCodeUpdateFormDto>> GetUpdateFormAsync(int campaignId, string userid);

        public Task<List<string>> GetCampaignChannelCodeList(int campaignId);
        public Task<string> GetCampaignChannelCodesAsString(int campaignId);
    }
}
