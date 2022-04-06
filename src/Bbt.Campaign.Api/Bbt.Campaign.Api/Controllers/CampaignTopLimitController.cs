using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.CampaignTopLimit;
using Bbt.Campaign.Services.Services.CampaignTopLimit;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignTopLimitController : BaseController<CampaignTopLimitController>
    {
        private readonly ICampaignTopLimitService _campaignTopLimitService;

        public CampaignTopLimitController(ICampaignTopLimitService campaignTopLimitService)
        {
            _campaignTopLimitService = campaignTopLimitService;
        }
        /// <summary>
        /// Returns the campaign top limit information by Id
        /// </summary>
        /// <param name="id">Campaign top limit Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adminSektor = await _campaignTopLimitService.GetCampaignTopLimitAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign top limit
        /// </summary>
        /// <param name="campaignTopLimit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignTopLimitInsertRequest campaignTopLimit)
        {
            var createResult = await _campaignTopLimitService.AddAsync(campaignTopLimit);
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign top limit by Id
        /// </summary>
        /// <param name="campaignTopLimit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignTopLimitUpdateRequest campaignTopLimit)
        {
            var result = await _campaignTopLimitService.UpdateAsync(campaignTopLimit);
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign top limit by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _campaignTopLimitService.DeleteAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignTopLimitService.GetListAsync();
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
            var result = await _campaignTopLimitService.GetInsertForm();
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int id)
        {
            var result = await _campaignTopLimitService.GetUpdateForm(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list for dropdown form data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-parameter-list")]
        public async Task<IActionResult> GetParameterList()
        {
            var result = await _campaignTopLimitService.GetFilterParameterList();
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign top limit list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CampaignTopLimitListFilterRequest request) 
        {
            var result = await _campaignTopLimitService.GetByFilterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign top limit excel list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter-excel")]
        public async Task<IActionResult> GetExcelAsync(CampaignTopLimitListFilterRequest request)
        {
            var result = await _campaignTopLimitService.GetExcelAsync(request);
            return Ok(result);
        }

    }
}
