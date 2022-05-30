using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Services.Services.Approval;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{
    public class ApproveController : BaseController<ApproveController>
    {
        private readonly IApprovalService _approvalService;

        public ApproveController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        /// <summary>
        /// Approves the draft campaign.
        /// </summary>
        /// <param name="id">Record Id of the draft</param>
        /// <returns></returns>
        [HttpGet]
        [Route("campaign/{id}")]
        public async Task<IActionResult> ApproveCampaign(int id, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _approvalService.ApproveCampaignAsync(id, userId);
            return Ok(result);
        }

        /// <summary>
        /// Approves the target by draft Id.
        /// </summary>
        /// <param name="id">Record Id of the draft</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("target/{id}")]
        //public async Task<IActionResult> ApproveTarget(int id)
        //{
        //    var result = await _approvalService.ApproveTargetAsync(id);
        //    return Ok(result);
        //}

        /// <summary>
        /// Approves the top limit by draft Id.
        /// </summary>
        /// <param name="id">Record Id of the draft</param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("toplimit/{id}")]
        //public async Task<IActionResult> ApproveTopLimit(int id)
        //{
        //    var result = await _approvalService.ApproveTopLimitAsync(id);
        //    return Ok(result);
        //}

        /// <summary>
        /// Returns the form data for campaign approval page
        /// </summary>
        /// <param name="id">Record Id of the draft campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-campaign-form")]
        public async Task<IActionResult> GetCampaignApprovalFormAsync(int id)
        {
            var result = await _approvalService.GetCampaignApprovalFormAsync(id);
            return Ok(result);
        }
        
        /// <summary>
        /// Returns the form data for target approval page
        /// </summary>
        /// <param name="id">Record Id of the draft target</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-target-approval-form")]
        public async Task<IActionResult> GetTargetApprovalFormAsync(int id)
        {
            var result = await _approvalService.GetTargetApprovalFormAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for top limit approval page
        /// </summary>
        /// <param name="id">Record Id of the draft top limit</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-toplimit-approval-form")]
        public async Task<IActionResult> GetTopLimitApprovalFormAsync(int id)
        {
            var result = await _approvalService.GetTopLimitApprovalFormAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for campaign view page
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("view-campaign")]
        public async Task<IActionResult> GetCampaignViewFormAsync(int campaignId)
        {
            var result = await _approvalService.GetCampaignViewFormAsync(campaignId);
            return Ok(result);
        }

        /// <summary>
        /// Copy the campaign to a new campaign 
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("copy-campaign")]
        public async Task<IActionResult> CampaignCopyAsync(int campaignId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _approvalService.CampaignCopyAsync(campaignId, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Copy the top limit information to a new top limit 
        /// </summary>
        /// <param name="topLimitId">Record Id of the top limit </param>
        /// <returns></returns>
        [HttpGet]
        [Route("copy-top-limit")]
        public async Task<IActionResult> TopLimitCopyAsync(int topLimitId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _approvalService.TopLimitCopyAsync(topLimitId, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Copy the top target to a new target
        /// </summary>
        /// <param name="targetId">Record Id of the target</param>
        /// <returns></returns>
        [HttpGet]
        [Route("copy-target")]
        public async Task<IActionResult> TargetCopyAsync(int targetId, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _approvalService.TargetCopyAsync(targetId, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }
    }
}
