using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
using Bbt.Campaign.Public.Models.CampaignIdentity;
using Bbt.Campaign.Services.Services.CampaignIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class CampaignIdentityController : BaseController<CampaignController>
    {
        private readonly ICampaignIdentityService _campaignIdentityService;

        public CampaignIdentityController(ICampaignIdentityService campaignIdentityService)
        {
            _campaignIdentityService = campaignIdentityService;
        }

        /// <summary>
        /// Updates campaign identities
        /// </summary>
        /// <param name="request">Campaign identitiy request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        //public async Task<IActionResult> Update([FromForm] UpdateCampaignIdentityRequest request)
        public async Task<IActionResult> Update(UpdateCampaignIdentityRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.UpdateAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Removes the campaign identity by Recor Id List.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(CampaignIdentityDeleteRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.DeleteAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign identity information by Id
        /// </summary>
        /// <param name="id">Campaign identity Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _campaignIdentityService.GetCampaignIdentityAsync(id);
            return Ok(adminSektor);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm()
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.GetUpdateFormAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CampaignIdentityListFilterRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.GetByFilterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign identity list excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignIdentityListFilterRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.GetByFilterExcelAsync(request);
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
