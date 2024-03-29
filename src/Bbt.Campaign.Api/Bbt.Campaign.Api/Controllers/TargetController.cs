﻿using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Target.Services.Services.Target;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Api.Extensions;

namespace Bbt.Target.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class TargetController : BaseController<TargetController>
    {
        private readonly ITargetService _targetService;

        public TargetController(ITargetService targetService)
        {
            _targetService = targetService;
        }

        /// <summary>
        /// Returns the target information by Id
        /// </summary>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _targetService.GetTargetAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new target
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(TargetInsertRequest Target)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var createResult = await _targetService.AddAsync(Target, await GetUser());
            return Ok(createResult);
        }
        /// <summary>
        /// Updates target by Id
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(TargetUpdateRequest Target)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetService.UpdateAsync(Target, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Removes the target by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Returns the target list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(TargetListFilterRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetService.GetByFilterAsync(request, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Returns the target list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-list-excel")]
        public async Task<IActionResult> GetExcelAsync(TargetListFilterRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetService.GetExcelAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the information for the target view form
        /// </summary>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-view-form")]
        public async Task<IActionResult> GetTargetViewFormAsync(int id)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetService.GetTargetViewFormAsync(id);
            return Ok(result);
        }

        private async Task<string> GetUser()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
            if (string.IsNullOrEmpty(userId))
                throw new Exception(ControllerStatics.UserNotFoundAlert);
            return userId;
        }
    }
}
