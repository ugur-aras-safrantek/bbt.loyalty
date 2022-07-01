using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Target.Detail;
using Bbt.Campaign.Services.Services.Target.Detail;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Api.Extensions;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]

    public class TargetDetailController : BaseController<TargetDetailController>
    {
        private readonly ITargetDetailService _targetDetailService;

        public TargetDetailController(ITargetDetailService targetDetailService)
        {
            _targetDetailService = targetDetailService;
        }
        /// <summary>
        /// Returns the target detail information by Id
        /// </summary>
        /// <param name="id">Target Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _targetDetailService.GetTargetDetailAsync(id);
            return Ok(adminSektor);
        }

        /// <summary>
        /// Returns the target detail information by Target Id
        /// </summary>
        /// <param name="targetId">Target Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get/{targetId}")]
        public async Task<IActionResult> GetByTarget(int targetId)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var adminSektor = await _targetDetailService.GetByTargetAsync(targetId);
            return Ok(adminSektor);
        }

        /// <summary>
        /// Adds new target detail
        /// </summary>
        /// <param name="TargetDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(TargetDetailInsertRequest TargetDetail)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var createResult = await _targetDetailService.AddAsync(TargetDetail, await GetUser());
            return Ok(createResult);
        }
        /// <summary>
        /// Updates target detail by Id
        /// </summary>
        /// <param name="TargetDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(TargetDetailUpdateRequest TargetDetail)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetDetailService.UpdateAsync(TargetDetail, await GetUser());
            return Ok(result);
        }
        /// <summary>
        /// Removes the target detail by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsInRole("IsLoyaltyRuleCreator"))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetDetailService.DeleteAsync(id);
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
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetDetailService.GetInsertFormAsync(await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int targetId)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetDetailService.GetUpdateFormAsync(targetId, await GetUser());
            return Ok(result);
        }



        /// <summary>
        /// Returns the target detail list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(TargetDetailListFilterRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _targetDetailService.GetByFilterAsync(request, await GetUser());
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
