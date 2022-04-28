

using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Models.Authorization;

namespace Bbt.Campaign.Services.Services.Authorization
{
    public interface IAuthorizationservice
    {
        public Task<BaseResponse<List<UserAuthorizationDto>>> LoginAsync(LoginRequest request);
    }
}
