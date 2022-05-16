using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Dtos.CampaignRule;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignRule;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignRuleController : BaseController<CampaignRuleController>
    {
        private readonly ICampaignRuleService _campaignRuleService;

        public CampaignRuleController(ICampaignRuleService campaignRuleService)
        {
            _campaignRuleService = campaignRuleService;
        }
        /// <summary>
        /// Returns the campaign rule information by Id
        /// </summary>
        /// <param name="id">Campaign Rule Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id, [FromHeader(Name = "userid")][Required] string userId)
        {
            var adminSektor = await _campaignRuleService.GetCampaignRuleAsync(id, General.GetUserIdFromHeader(Request));
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign rule
        /// </summary>
        /// <param name="campaignRule">Campaign Rule</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        //public async Task<IActionResult> Add([FromForm] AddCampaignRuleRequest campaignRule)
        public async Task<IActionResult> Add(AddCampaignRuleRequest campaignRule, [FromHeader(Name = "userid")][Required] string userId)
        {
            var createResult = await _campaignRuleService.AddAsync(campaignRule, General.GetUserIdFromHeader(Request));
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign rule by Id
        /// </summary>
        /// <param name="campaignRule">Campaign Rule</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        //public async Task<IActionResult> Update([FromForm] UpdateCampaignRuleRequest campaignRule)
        public async Task<IActionResult> Update(UpdateCampaignRuleRequest campaignRule, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignRuleService.UpdateAsync(campaignRule, General.GetUserIdFromHeader(this.Request));
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign rule by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignRuleService.DeleteAsync(id, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign rule list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignRuleService.GetListAsync();
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
            var result = await _campaignRuleService.GetInsertForm(userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId">Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _campaignRuleService.GetUpdateForm(campaignId, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign rule identity file data  
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-identity-file")]
        public async Task<IActionResult> GetRuleIdentityFileAsync(int campaignId)
        {
            var result = await _campaignRuleService.GetRuleIdentityFileAsync(campaignId);
            return Ok(result);
        }
    }
}
