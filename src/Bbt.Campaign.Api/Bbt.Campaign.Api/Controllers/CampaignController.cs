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
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        //[HttpGet("{id}")]
        [HttpGet]
        [Route("get/{id}")]
        public async Task<IActionResult> GetById(int id, string userid)
        {
            var adminSektor = await _campaignService.GetCampaignAsync(id, userid);
            return Ok(adminSektor);
        }
        /// <summary>
        /// Adds new campaign
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add(CampaignInsertRequest campaign, string userid)
        {
            var createResult = await _campaignService.AddAsync(campaign, userid);
            return Ok(createResult);
        }
        /// <summary>
        /// Updates campaign by Id
        /// </summary>
        /// <param name="campaign"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update(CampaignUpdateRequest campaign, string userid)
        {
            var result = await _campaignService.UpdateAsync(campaign, userid);
            return Ok(result);
        }
        /// <summary>
        /// Removes the campaign by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> Delete(int id, string userid)
        {
            var result = await _campaignService.DeleteAsync(id, userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list
        /// </summary> 
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("getall")]
        public async Task<IActionResult> GetList(string userid)
        {
            var result = await _campaignService.GetListAsync(userid);
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
            var result = await _campaignService.GetInsertFormAsync(userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the form data for update page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-update-form")]
        public async Task<IActionResult> GetUpdateForm(int id, string userid)
        {
            var result = await _campaignService.GetUpdateFormAsync(id, _webHostEnvironment.ContentRootPath, userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter")]
        public async Task<IActionResult> GetByFilter(CampaignListFilterRequest request, string userid)
        {
            var result = await _campaignService.GetByFilterAsync(request, userid);
            return Ok(result);
        }
        /// <summary>
        /// Returns the campaign list excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userid">User Id</param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignListFilterRequest request, string userid)
        {
            var result = await _campaignService.GetByFilterExcelAsync(request, userid);
            return Ok(result);
        }

        /// <summary>
        /// Returns the contract data for 
        /// </summary>
        /// <param name="id">Contract Id</param>
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
