using AutoMapper;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignChannelCode;
using Bbt.Campaign.Public.Models.CampaignChannelCode;

namespace Bbt.Campaign.Services.Services.CampaignChannelCode
{
    public interface ICampaignChannelCodeService
    {
        public Task<BaseResponse<CampaignChannelCodeDto>> UpdateAsync(CampaignChannelCodeUpdateRequest request);

        public Task<BaseResponse<CampaignChannelCodeInsertFormDto>> GetInsertFormAsync(int campaignId);

        public Task<BaseResponse<CampaignChannelCodeUpdateFormDto>> GetUpdateFormAsync(int campaignId);

        public Task<List<string>> GetCampaignChannelCodeList(int campaignId);
    }
}
