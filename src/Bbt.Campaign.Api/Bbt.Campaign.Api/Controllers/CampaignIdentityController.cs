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
        /// Returns the form data for update page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm()
        {
            //if (!User.IsInRole("isLoyaltyCreator"))
            //    throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

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
            //if (!User.IsInRole("isLoyaltyCreator"))
            //    throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignIdentityService.GetByFilterAsync(request);
            return Ok(result);
        }
    }
}
