﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignIdentity;
using Bbt.Campaign.Public.Models.CampaignIdentity;
using Bbt.Campaign.Public.Models.File;
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
        public Task<BaseResponse<List<CampaignIdentityDto>>> DeleteAsync(CampaignIdentityDeleteRequest request, string userId);
        public Task<BaseResponse<CampaignIdentityDto>> GetCampaignIdentityAsync(int id);
        public Task<BaseResponse<CampaignIdentityUpdateFormDto>> GetUpdateFormAsync();
        public Task<BaseResponse<CampaignIdentityListFilterResponse>> GetByFilterAsync(CampaignIdentityListFilterRequest request);
        public Task<BaseResponse<GetFileResponse>> GetByFilterExcelAsync(CampaignIdentityListFilterRequest request);
    }
}
