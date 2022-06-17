using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Target.Services.Services.Target;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;

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
            var result = await _targetService.GetTargetViewFormAsync(id);
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
