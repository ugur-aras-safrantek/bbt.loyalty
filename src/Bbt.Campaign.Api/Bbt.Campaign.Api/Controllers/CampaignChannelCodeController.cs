using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
using Bbt.Campaign.Services.Services.CampaignChannelCode;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignChannelCodeController : BaseController<CampaignChannelCodeController>
    {
        private readonly ICampaignChannelCodeService _campaignChannelCodeService;

        public CampaignChannelCodeController(ICampaignChannelCodeService campaignChannelCodeService)
        {
            _campaignChannelCodeService = campaignChannelCodeService;
        }

        /// <summary>
        /// Updates campaign channel code list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignChannelCodeUpdateRequest request)
        {
            var result = await _campaignChannelCodeService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm(int campaignId)
        {
            var result = await _campaignChannelCodeService.GetInsertFormAsync(campaignId);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId)
        {
            var result = await _campaignChannelCodeService.GetUpdateFormAsync(campaignId);
            return Ok(result);
        }

    }
}
