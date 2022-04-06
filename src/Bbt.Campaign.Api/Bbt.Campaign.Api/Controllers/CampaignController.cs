using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Services.Services.Campaign;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignController : BaseController<CampaignController>
    {
        private readonly ICampaignService _campaignService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CampaignController(ICampaignService campaignService, IWebHostEnvironment webHostEnvironment)
        {
            _campaignService = campaignService;
            _webHostEnvironment = webHostEnvironment;
        }


        /// <summary>
        /// Returns the campaign information by Id
        /// </summary>
        /// <param name="id">Campaign Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var adminSektor = await _campaignService.GetCampaignAsync(id);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign
        /// </summary>
        /// <param name="campaign"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignInsertRequest campaign)
        {
            var createResult = await _campaignService.AddAsync(campaign);
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign by Id
        /// </summary>
        /// <param name="campaign"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignUpdateRequest campaign)
        {
            var result = await _campaignService.UpdateAsync(campaign);
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _campaignService.DeleteAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await _campaignService.GetListAsync();
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
            var result = await _campaignService.GetInsertFormAsync();
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
            var result = await _campaignService.GetUpdateFormAsync(id);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CampaignListFilterRequest request)
        {
            var result = await _campaignService.GetByFilterAsync(request);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignListFilterRequest request)
        {
            var result = await _campaignService.GetByFilterExcelAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="id">Campaign Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-contract-file")]
        public async Task<IActionResult> GetContractFileAsync(int id)
        {
            var result = await _campaignService.GetContractFileAsync(id, _webHostEnvironment.ContentRootPath);
            return Ok(result);
        }
    }
}
