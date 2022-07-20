using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignIdentity;
using Bbt.Campaign.Public.Models.CampaignIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Services.Services.CampaignIdentity
{
    public interface ICampaignIdentityService
    {
        public Task<BaseResponse<List<CampaignIdentityDto>>> UpdateAsync(UpdateCampaignIdentityRequest request, string userId);
        public Task<BaseResponse<CampaignIdentityUpdateFormDto>> GetUpdateFormAsync();
        public Task<BaseResponse<CampaignIdentityListFilterResponse>> GetByFilterAsync(CampaignIdentityListFilterRequest request);
    }
}
