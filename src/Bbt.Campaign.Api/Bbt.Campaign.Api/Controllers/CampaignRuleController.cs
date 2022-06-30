using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignRule;
using Bbt.Campaign.Services.Services.CampaignRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
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
        public async Task<IActionResult> GetById(int id)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _campaignRuleService.GetCampaignRuleAsync(id);
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
        public async Task<IActionResult> Add(AddCampaignRuleRequest campaignRule)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var createResult = await _campaignRuleService.AddAsync(campaignRule, await GetUser());
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
        public async Task<IActionResult> Update(UpdateCampaignRuleRequest campaignRule)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignRuleService.UpdateAsync(campaignRule, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign rule by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignRuleService.DeleteAsync(id, await GetUser());
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
        public async Task<IActionResult> GetInsertForm()
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignRuleService.GetInsertForm(await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId">Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignRuleService.GetUpdateForm(campaignId, await GetUser());
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
