using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Services.Services.CampaignAchievement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class CampainAchievementController : BaseController<CampainAchievementController>
    {
        private readonly ICampaignAchievementService _campaignAchievementService;

        public CampainAchievementController(ICampaignAchievementService campaignAchievementService)
        {
            _campaignAchievementService = campaignAchievementService;
        }
        /// <summary>
        /// Returns the campaign Achievement information by Id
        /// </summary>
        /// <param name="id">Campaign Achievement Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adminSektor = await _campaignAchievementService.GetCampaignAchievementAsync(id);
            return Ok(adminSektor);
        }

        /// <summary>
        /// Adds new campaign Achievement
        /// </summary>
        /// <param name="campaignAchievement"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignAchievementInsertRequest campaignAchievement)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.UpdateAsync(campaignAchievement, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Updates campaign Achievement by Id
        /// </summary>
        /// <param name="campaignAchievement"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignAchievementInsertRequest campaignAchievement)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.UpdateAsync(campaignAchievement, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign Achievement by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.DeleteAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign Achievement list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignAchievementService.GetListAsync();
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
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.GetInsertFormAsync(campaignId, await GetUser());
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
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.GetUpdateFormAsync(campaignId, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// sends campaign for the approval
        /// </summary>
        /// <param name="campaignId">Record Id of the capaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("send-to-approval")]
        public async Task<IActionResult> SendToAppropval(int campaignId)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignAchievementService.SendToAppropval(campaignId, await GetUser());
            return Ok(result);
        }


        ///// <summary>
        ///// Returns the campaign Achievement list by campaign Id
        ///// </summary>
        ///// <returns>returns as gropupped by channels</returns>
        //[HttpGet]
        //[Route("get-list-by-campaign")]
        //public async Task<IActionResult> GetListByCampaign(int campaignId)
        //{
        //    var result = await _campaignAchievementService.GetListByCampaignAsync(campaignId);
        //    return Ok(result);
        //}


        private async Task<UserRoleDto> GetUser()
        {
            UserRoleDto userRoleDto2 = new UserRoleDto();

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
