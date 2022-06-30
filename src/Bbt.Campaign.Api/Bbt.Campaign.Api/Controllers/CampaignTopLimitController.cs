using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignTopLimit;
using Bbt.Campaign.Services.Services.CampaignTopLimit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class CampaignTopLimitController : BaseController<CampaignTopLimitController>
    {
        

        private readonly ICampaignTopLimitService _campaignTopLimitService;

        public CampaignTopLimitController(ICampaignTopLimitService campaignTopLimitService)
        {
            _campaignTopLimitService = campaignTopLimitService;
        }
        /// <summary>
        /// Returns the campaign top limit information by Id
        /// </summary>
        /// <param name="id">Campaign top limit Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _campaignTopLimitService.GetCampaignTopLimitAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign top limit
        /// </summary>
        /// <param name="campaignTopLimit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignTopLimitInsertRequest campaignTopLimit)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var createResult = await _campaignTopLimitService.AddAsync(campaignTopLimit, await GetUser());
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign top limit by Id
        /// </summary>
        /// <param name="campaignTopLimit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignTopLimitUpdateRequest campaignTopLimit)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.UpdateAsync(campaignTopLimit, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign top limit by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.DeleteAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.GetListAsync();
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm()
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.GetInsertForm(await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int id)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.GetUpdateForm(id, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list for dropdown form data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-parameter-list")]
        public async Task<IActionResult> GetParameterList()
        {
            var result = await _campaignTopLimitService.GetFilterParameterList();
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CampaignTopLimitListFilterRequest request) 
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.GetByFilterAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign top limit excel list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter-excel")]
        public async Task<IActionResult> GetExcelAsync(CampaignTopLimitListFilterRequest request)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTopLimitService.GetExcelAsync(request, await GetUser());
            return Ok(result);
        }

        private async Task<string> GetUser()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception(ControllerStatics.UserNotFoundAlert);
            return userId;
        }

    }
}
