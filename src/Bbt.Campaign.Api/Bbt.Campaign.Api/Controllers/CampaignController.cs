using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Campaign;
using Bbt.Campaign.Services.Services.Campaign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Api.Controllers
{

    [Authorize]
    [Route("[controller]")]
    [ApiController]
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
            var createResult = await _campaignService.AddAsync(campaign, await GetUser());
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
            var result = await _campaignService.UpdateAsync(campaign, await GetUser());
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
            var result = await _campaignService.DeleteAsync(id, await GetUser());
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
            var result = await _campaignService.GetListAsync(await GetUser());
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
            var result = await _campaignService.GetInsertFormAsync(await GetUser());
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
            var result = await _campaignService.GetUpdateFormAsync(id, _webHostEnvironment.ContentRootPath, await GetUser());
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
            string userid = General.GetUserIdFromHeader(Request);

            var result = await _campaignService.GetByFilterAsync(request, await GetUser());
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
            var result = await _campaignService.GetByFilterExcelAsync(request, await GetUser());
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

        /// <summary>
        /// creates the draft of the campaign
        /// </summary>
        /// <param name="campaignId">Record Id of the campaign</param>
        /// <returns></returns>
        [HttpGet]
        [Route("create-draft")]
        public async Task<IActionResult> CreateDraftAsync(int campaignId)
        {
            var result = await _campaignService.CreateDraftAsync(campaignId, await GetUser());
            return Ok(result);
        }

        private async Task<UserRoleDto2> GetUser() 
        {
            UserRoleDto2 userRoleDto2 = new UserRoleDto2();

            List<int> roleTypeIdList = new List<int>();
            userRoleDto2.UserId = User.Claims.FirstOrDefault(c => c.Type == "UserId").Value;

            if (Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyCreator").Value))
                roleTypeIdList.Add((int)RoleTypeEnum.IsLoyaltyCreator);

            if(Convert.ToBoolean(User.Claims.FirstOrDefault(c => c.Type == "IsLoyaltyApprover").Value))
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
