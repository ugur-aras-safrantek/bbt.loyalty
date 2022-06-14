using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Services.Services.Approval;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
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
        /// <param name="id">Record Id of the draft campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("campaign/{id}")]
        public async Task<IActionResult> ApproveCampaign(int id)
        {
            var result = await _approvalService.ApproveCampaignAsync(id, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// DisApproves the draft campaign.
        /// </summary>
        /// <param name="id">Record Id of the draft campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("campaign-disapprove/{id}")]
        public async Task<IActionResult> DisApproveCampaign(int id)
        {
            var result = await _approvalService.DisApproveCampaignAsync(id, await GetUser());
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
            var result = await _approvalService.GetCampaignApprovalFormAsync(id, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for target approval page
        /// </summary>
        /// <param name="id">Record Id of the draft target</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-target-form")]
        public async Task<IActionResult> GetTargetApprovalFormAsync(int id)
        {
            var result = await _approvalService.GetTargetApprovalFormAsync(id, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Approves or disapproves the draft target.
        /// </summary>
        /// <param name="id">Record Id of the draft target</param>
        /// <param name="isApproved">Approved or disapproved</param>
        /// <returns></returns>
        [HttpPost]
        [Route("target/{id}/{isApproved}")]
        public async Task<IActionResult> ApproveTargetAsync(int id, bool isApproved)
        {
            var result = await _approvalService.ApproveTargetAsync(id, isApproved, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for top limit approval page
        /// </summary>
        /// <param name="id">Record Id of the draft top limit</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-toplimit-form")]
        public async Task<IActionResult> GetTopLimitApprovalFormAsync(int id)
        {
            var result = await _approvalService.GetTopLimitApprovalFormAsync(id, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Approves or disapproves the draft toplimit.
        /// </summary>
        /// <param name="id">Record Id of the draft toplimit</param>
        /// <param name="isApproved">Approved or disapproved</param>
        /// <returns></returns>
        [HttpPost]
        [Route("toplimit/{id}/{isApproved}")]
        public async Task<IActionResult> ApproveTopLimit(int id, bool isApproved)
        {
            var result = await _approvalService.ApproveTopLimitAsync(id, isApproved, await GetUser());
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
        public async Task<IActionResult> CampaignCopyAsync(int campaignId)
        {
            var result = await _approvalService.CampaignCopyAsync(campaignId, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Copy the top limit information to a new top limit 
        /// </summary>
        /// <param name="topLimitId">Record Id of the top limit </param>
        /// <returns></returns>
        [HttpGet]
        [Route("copy-top-limit")]
        public async Task<IActionResult> TopLimitCopyAsync(int topLimitId)
        {
            var result = await _approvalService.TopLimitCopyAsync(topLimitId, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Copy the top target to a new target
        /// </summary>
        /// <param name="targetId">Record Id of the target</param>
        /// <returns></returns>
        [HttpGet]
        [Route("copy-target")]
        public async Task<IActionResult> TargetCopyAsync(int targetId)
        {
            var result = await _approvalService.TargetCopyAsync(targetId, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// convert
        /// </summary>
        /// <param name="date"></param>
        /// /// <param name="format"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConvertWithInvariantCulture")]
        public async Task<IActionResult> ConvertWithInvariantCulture(string date, string format)
        {
            var result = _approvalService.ConvertWithInvariantCulture(date, format);
            return Ok(result);
        }

        /// <summary>
        /// convert
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConvertWithCulture")]
        public async Task<IActionResult> ConvertWithCulture(string date, string format, string culture)
        {
            var result = _approvalService.ConvertWithCulture(date, format, culture);
            return Ok(result);
        }

        /// <summary>
        /// convert
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ConvertWithNewDateTime")]
        public async Task<IActionResult> ConvertWithNewDateTime(string date)
        {
            var result = _approvalService.ConvertWithNewDateTime(date);
            return Ok(result);
        }

        private async Task<UserRoleDto2> GetUser()
        {
            UserRoleDto2 userRoleDto2 = new UserRoleDto2();

            List<int> roleTypeIdList = new List<int>();
            userRoleDto2.UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyCreator").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyCreator);

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyApprover").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyApprover);

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyReader").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyReader);

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyRuleCreator").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyRuleCreator);

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyRuleApprover").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyRuleApprover);

            if (!roleTypeIdList.Any())
                throw new Exception("Kullanıcının yetkisi yoktur.");

            userRoleDto2.RoleTypeIdList = roleTypeIdList;

            return userRoleDto2;
        }

    }
}
