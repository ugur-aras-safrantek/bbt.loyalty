using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Campaign;

namespace Bbt.Campaign.Services.Services.Draft
{
    public interface IDraftService
    {
        public Task<int> CreateCampaignDraftAsync(int campaignId, string userid);
        public Task<BaseResponse<CampaignDto>> CreateCampaignCopyAsync(int campaignId, string userid);
        public Task<CampaignProperty> GetCampaignProperties(int campaignId);
        public Task<int> GetProcessType(int canpaignId);
    }
}
