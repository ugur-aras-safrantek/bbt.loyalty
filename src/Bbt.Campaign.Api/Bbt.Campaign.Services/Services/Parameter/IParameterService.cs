using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;

namespace Bbt.Campaign.Services.Services.Parameter
{
    public interface IParameterService
    {
        public Task<BaseResponse<List<ParameterDto>>> GetActionOptionListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetBranchListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetBusinessLineListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetCampaignStartTermListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetCustomerTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetJoinTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetLanguageListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetSectorListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetViewOptionListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetProgramTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetAchievementFrequencyListAsync();
        public Task<BaseResponse<List<string>>> GetCampaignChannelListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetCurrencyListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetTargetDefinitionListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetTargetOperationListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetTargetSourceListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetTargetViewTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetTriggerTimeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetVerificationTimeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetAchievementTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetParticipationTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetRoleTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetModuleTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetAuthorizationTypeListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetAllUsersRoleListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetAllUsersRoleListInProgressAsync(string cacheKey);
        public Task<BaseResponse<List<ParameterDto>>> GetSingleUserRoleListAsync(string userId);
        public Task<BaseResponse<List<RoleAuthorizationDto>>> GetRoleAuthorizationListAsync();
        public Task<BaseResponse<List<UserRoleDto>>> GetUserRoleListAsync(string userId);
        public Task<BaseResponse<List<UserRoleDto>>> SetUserRoleListAsync(string userId, List<UserRoleDto> _userRoleList);
        public Task<BaseResponse<List<ParameterDto>>> GetBranchSelectDateListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetChannelCodeSelectDateListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetServiceConstantListAsync();
        public Task<BaseResponse<List<ParameterDto>>> GetStatusListAsync();
        
        
        public Task<string> GetServiceData(string serviceUrl);
        public Task<string> GetServiceConstantValue(string code);
    }
}
