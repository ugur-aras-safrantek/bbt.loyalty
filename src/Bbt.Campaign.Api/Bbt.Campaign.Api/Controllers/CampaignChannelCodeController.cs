using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.CampaignChannelCode;
using Bbt.Campaign.Services.Services.CampaignChannelCode;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        /// Adds new campaign channel code list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignChannelCodeUpdateRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignChannelCodeService.UpdateAsync(request, Request.Headers["userid"].ToString());
            return Ok(result);
        }


        /// <summary>
        /// Updates campaign channel code list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignChannelCodeUpdateRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignChannelCodeService.UpdateAsync(request, Request.Headers["userid"].ToString());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm(int campaignId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignChannelCodeService.GetInsertFormAsync(campaignId, Request.Headers["userid"].ToString());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignChannelCodeService.GetUpdateFormAsync(campaignId, Request.Headers["userid"].ToString());
            return Ok(result);
        }

    }
}
