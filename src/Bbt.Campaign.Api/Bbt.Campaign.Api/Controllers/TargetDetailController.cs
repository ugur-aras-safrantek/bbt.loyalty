using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Target.Detail;
using Bbt.Campaign.Services.Services.Target.Detail;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
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
            var adminSektor = await _targetDetailService.GetByTargetAsync(targetId);
            return Ok(adminSektor);
        }

        /// <summary>
        /// Adds new target detail
        /// </summary>
        /// <param name="TargetDetail"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(TargetDetailInsertRequest TargetDetail, string userid)
        {
            var createResult = await _targetDetailService.AddAsync(TargetDetail, userid);
            return Ok(createResult);
        }
        /// <summary>
        /// Updates target detail by Id
        /// </summary>
        /// <param name="TargetDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(TargetDetailUpdateRequest TargetDetail, string userid)
        {
            var result = await _targetDetailService.UpdateAsync(TargetDetail, userid);
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
            var result = await _targetDetailService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for insert page
        /// </summary>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-insert-form")]
        public async Task<IActionResult> GetInsertForm(string userid)
        {
            var result = await _targetDetailService.GetInsertFormAsync(userid);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int targetId, string userid)
        {
            var result = await _targetDetailService.GetUpdateFormAsync(targetId, userid);
            return Ok(result);
        }



        /// <summary>
        /// Returns the target detail list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(TargetDetailListFilterRequest request, string userid)
        {
            var result = await _targetDetailService.GetByFilterAsync(request, userid);
            return Ok(result);
        }
    }
}
