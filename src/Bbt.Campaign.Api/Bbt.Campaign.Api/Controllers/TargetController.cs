using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Target.Services.Services.Target;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Target.Api.Controllers
{
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
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(TargetInsertRequest Target, string userid)
        {
            var createResult = await _targetService.AddAsync(Target, userid);
            return Ok(createResult);
        }
        /// <summary>
        /// Updates target by Id
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(TargetUpdateRequest Target, string userid)
        {
            var result = await _targetService.UpdateAsync(Target, userid);
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
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(TargetListFilterRequest request, string userid)
        {
            var result = await _targetService.GetByFilterAsync(request, userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the target list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-list-excel")]
        public async Task<IActionResult> GetExcelAsync(TargetListFilterRequest request, string userid)
        {
            var result = await _targetService.GetExcelAsync(request, userid);
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
    }
}
