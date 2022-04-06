﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Models.CampaignTopLimit;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Services.Services.CampaignTopLimit
{
    public interface ICampaignTopLimitService
    {
        public Task<BaseResponse<TopLimitDto>> GetCampaignTopLimitAsync(int id);
        public Task<BaseResponse<TopLimitDto>> AddAsync(CampaignTopLimitInsertRequest campaignTopLimit);
        public Task<BaseResponse<TopLimitDto>> UpdateAsync(CampaignTopLimitUpdateRequest campaignTopLimit);
        public Task<BaseResponse<List<TopLimitDto>>> GetListAsync();
        public Task<BaseResponse<TopLimitDto>> DeleteAsync(int id);
        public Task<BaseResponse<CampaignTopLimitInsertFormDto>> GetInsertForm();
        public Task<BaseResponse<CampaignTopLimitFilterParameterResponse>> GetFilterParameterList();
        public Task<BaseResponse<CampaignTopLimitUpdateFormDto>> GetUpdateForm(int id);
        public Task<BaseResponse<CampaignTopLimitListFilterResponse>> GetByFilterAsync(CampaignTopLimitListFilterRequest request);
        public Task<BaseResponse<GetFileResponse>> GetExcelAsync(CampaignTopLimitListFilterRequest request);

    }
}
