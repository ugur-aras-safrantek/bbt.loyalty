﻿using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.CampaignTarget;
using Bbt.Campaign.Services.Services.CampaignTarget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
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
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

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
        public async Task<IActionResult> Add(CampaignTargetInsertRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var createResult = await _campaignTargetService.UpdateAsync(request, await GetUser());
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign targets
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignTargetInsertRequest request)
        {
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTargetService.UpdateAsync(request, await GetUser());
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
            if (!User.IsInRole("IsLoyaltyCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

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
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

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
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTargetService.GetListByCampaignAsync(campaignId);
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

            var result = await _campaignTargetService.GetInsertForm(await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int campaignId)
        {
            if (!User.IsInRole("IsLoyaltyReader"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _campaignTargetService.GetUpdateForm(campaignId, await GetUser());
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
