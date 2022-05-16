using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.CampaignTarget;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignTargetController : BaseController<CampaignTargetController>
    {
        private readonly ICampaignTargetService _campaignTargetService;

        public CampaignTargetController(ICampaignTargetService campaignTargetService)
        {
            _campaignTargetService = campaignTargetService;
        }
        /// <summary>
        /// Returns the campaign target information by Id
        /// </summary>
        /// <param name="id">Campaign target Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adminSektor = await _campaignTargetService.GetCampaignTargetAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds campaign targets.(Put 0 between groups)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignTargetInsertRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var createResult = await _campaignTargetService.UpdateAsync(request, Request.Headers["userid"].ToString());
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign targets
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignTargetInsertRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignTargetService.UpdateAsync(request, Request.Headers["userid"].ToString());
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign target by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _campaignTargetService.DeleteAsync(id);
            return Ok(result);
        }
                
        /// <summary>
        /// Returns the campaign target list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignTargetService.GetListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign targets by campaignId
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-by-campaign")]
        public async Task<IActionResult> GetListByCampaign(int campaignId)
        {
            var result = await _campaignTargetService.GetListByCampaignAsync(campaignId);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm(string userid)
        {
            var result = await _campaignTargetService.GetInsertForm(userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignTargetService.GetUpdateForm(campaignId, Request.Headers["userid"].ToString());
            return Ok(result);
        }
    }
}
