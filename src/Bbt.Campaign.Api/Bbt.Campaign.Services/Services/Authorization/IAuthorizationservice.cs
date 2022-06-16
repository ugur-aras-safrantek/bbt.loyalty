﻿

using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;
using Microsoft.Extensions.Configuration;

namespace Bbt.Campaign.Services.Services.Authorization
{
    public interface IAuthorizationService
    {
        public Task<BaseResponse<UserAuthorizationResponseDto>> LoginAsync(string code, string state);
        //public Task<BaseResponse<CheckAuthorizationResponse>> CheckAuthorizationAsync(CheckAuthorizationRequest request);
        //public Task CheckAuthorizationAsync(string userId, int moduleTypeId, int authorizationTypeId);
        public Task CheckAuthorizationAsync(UserRoleDto userRoleDto, int moduleTypeId, int authorizationTypeId);
    }
}
