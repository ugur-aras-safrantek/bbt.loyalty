using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Services.Services.Cache;
using Bbt.Campaign.Services.Services.Parameter;
using Microsoft.AspNetCore.Mvc;
namespace Bbt.Campaign.Api.Controllers
{
    public class ParameterController : BaseController<ParameterController>
    {
        private readonly IParameterService _parameterService;
        private readonly ICacheServis _cacheService;

        public ParameterController(IParameterService parameterService, ICacheServis cacheService)
        {
            _parameterService = parameterService;
            _cacheService = cacheService;
        }
        /// <summary>
        /// Returns Action Option list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-action-options")]
        public async Task<IActionResult> GetActionOptionList()
        {
            var result = await _parameterService.GetActionOptionListAsync();
            return Ok(result);
        }
        
        /// <summary>
        /// Returns Business Line list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-business-lines")]
        public async Task<IActionResult> GetBusinessLineList()
        {
            var result = await _parameterService.GetBusinessLineListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Campaign Start Term list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-campaign-startterms")]
        public async Task<IActionResult> GetCampaignStartTermList()
        {
            var result = await _parameterService.GetCampaignStartTermListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Customer Type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-customer-types")]
        public async Task<IActionResult> GetCustomerTypeList()
        {
            var result = await _parameterService.GetCustomerTypeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Join Type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-join-types")]
        public async Task<IActionResult> GetJoinTypeList()
        {
            var result = await _parameterService.GetJoinTypeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Language list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-languages")]
        public async Task<IActionResult> GetLanguageList()
        {
            var result = await _parameterService.GetLanguageListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Sector list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-sectors")]
        public async Task<IActionResult> GetSectorList()
        {
            var result = await _parameterService.GetSectorListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns View Option list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-view-options")]
        public async Task<IActionResult> GetViewOptionList()
        {
            var result = await _parameterService.GetViewOptionListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Program Type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-program-types")]
        public async Task<IActionResult> GetProgramTypeList()
        {
            var result = await _parameterService.GetProgramTypeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Trigger Type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-trigger-times")]
        public async Task<IActionResult> GetTriggerTimeList()
        {
            var result = await _parameterService.GetTriggerTimeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Verification Time list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-verification-times")]
        public async Task<IActionResult> GetVerificationTimeList()
        {
            var result = await _parameterService.GetVerificationTimeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Target Source list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-target-sources")]
        public async Task<IActionResult> GetTargetSourceList()
        {
            var result = await _parameterService.GetTargetSourceListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Target View list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-target-views")]
        public async Task<IActionResult> GetTargetViewTypeList()
        {
            var result = await _parameterService.GetTargetViewTypeListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns Achievement Type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-achievement-types")]
        public async Task<IActionResult> GetAchievementTypeList()
        {
            var result = await _parameterService.GetAchievementTypeListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns Achievement Frequency list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-achievement-frequencies")]
        public async Task<IActionResult> GetAchievementFrequencyList()
        {
            var result = await _parameterService.GetAchievementFrequencyListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns Join participation type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-participation-types")]
        public async Task<IActionResult> GetParticipationTypeList()
        {
            var result = await _parameterService.GetParticipationTypeListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns role type list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-role-types")]
        public async Task<IActionResult> GetRoleTypeListAsync()
        {
            var result = await _parameterService.GetRoleTypeListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns role module list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-module-types")]
        public async Task<IActionResult> GetModuleTypeListAsync()
        {
            var result = await _parameterService.GetModuleTypeListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns role process list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-process-types")]
        public async Task<IActionResult> GetProcessTypeListAsync()
        {
            var result = await _parameterService.GetProcessTypeListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the roles of the all users.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-all-users-role-list")]
        public async Task<IActionResult> GetAllUsersRoleListAsync()
        {
            var result = await _parameterService.GetAllUsersRoleListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the roles of the all users.
        /// </summary>
        /// <param name="userId">Record Id of the user</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-single-user-role-list")]
        public async Task<IActionResult> GetSingleUserRoleListAsync(string userId)
        {
            var result = await _parameterService.GetSingleUserRoleListAsync(userId);
            return Ok(result);
        }



        /// <summary>
        /// Returns the role authorization list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-role-authorization-list")]
        public async Task<IActionResult> GetRoleAuthorizationListAsync()
        {
            var result = await _parameterService.GetRoleAuthorizationListAsync();
            return Ok(result);
        }


        /// <summary>
        /// Returns Branch list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-branchs")]
        public async Task<IActionResult> GetBranchList()
        {
            var result = await _parameterService.GetBranchListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns channel code list.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-channel-codes")]
        public async Task<IActionResult> GetCampaignChannelListAsync()
        {
            var result = await _parameterService.GetCampaignChannelListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the last branch select date
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-branch-selectdate")]
        public async Task<IActionResult> GetBranchSelectDateList()
        {
            var result = await _parameterService.GetBranchSelectDateListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the last channel code select date
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-channelcode-selectdate")]
        public async Task<IActionResult> GetChannelCodeSelectDateListAsync()
        {
            var result = await _parameterService.GetChannelCodeSelectDateListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Clears all keys on the cache.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("redis-clear")]
        public async Task<IActionResult> ClearCacheRedis()
        {
            var result = await _cacheService.ClearCacheRedis();
            return Ok(result);
        }

        /// <summary>
        /// Gets the service data given by the url
        /// </summary>
        /// <param name="url">Service url</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-service-data")]
        public async Task<IActionResult> GetServiceData( string url)
        {
            var result = await _parameterService.GetServiceData(url);
            return Ok(result);
        }
    }
}
