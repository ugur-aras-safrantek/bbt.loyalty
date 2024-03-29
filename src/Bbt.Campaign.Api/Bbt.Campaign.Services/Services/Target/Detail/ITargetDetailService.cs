﻿using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Models.Target.Detail;

namespace Bbt.Campaign.Services.Services.Target.Detail
{
    public interface ITargetDetailService
    {
        public Task<BaseResponse<TargetDetailDto>> AddAsync(TargetDetailInsertRequest targetDetail, string userId);
        public Task<BaseResponse<TargetDetailDto>> UpdateAsync(TargetDetailInsertRequest targetDetail, string userId);
        public Task<BaseResponse<TargetDetailListFilterResponse>> GetByFilterAsync(TargetDetailListFilterRequest request, string userId);
        public Task<BaseResponse<TargetDetailDto>> DeleteAsync(int id);
        public Task<BaseResponse<TargetDetailDto>> GetTargetDetailAsync(int id);
        public Task<BaseResponse<TargetDetailDto>> GetByTargetAsync(int targetId);
        public Task<BaseResponse<TargetDetailInsertFormDto>> GetInsertFormAsync(string userId);
        public Task<BaseResponse<TargetDetailUpdateFormDto>> GetUpdateFormAsync(int targetId, string userId);
        public Task<TargetDetailDto> GetTargetDetailDto(int targetId);
    }
}
