using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Dtos.Authorization;
using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Report;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Bbt.Campaign.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class ReportController : BaseController<ReportController>
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Returns the form data for campaign report form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-campaign-report-form")]
        public async Task<IActionResult> FillCampaignFormAsync()
        {
            var result = await _reportService.FillCampaignFormAsync(await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaignreport-by-filter")]
        public async Task<IActionResult> GetCampaignReportByFilterAsync(CampaignReportRequest request)
        {
            var result = await _reportService.GetCampaignReportByFilterAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaign-report-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignReportRequest request)
        {
            var result = await _reportService.GetCampaignReportExcelAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer report form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-customer-report-form")]
        public async Task<IActionResult> FillCustomerFormAsync()
        {
            var result = await _reportService.FillCustomerFormAsync(await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter")]
        public async Task<IActionResult> GetCustomerReportByFilterAsync(CustomerReportRequest request)
        {
            var result = await _reportService.GetCustomerReportByFilterAsync(request, await GetUser());
            return Ok(result);
        }


        /// <summary>
        /// Returns the customer report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter-excel")]
        public async Task<IActionResult> GetCustomerReportExcelAsync(CustomerReportRequest request)
        {
            var result = await _reportService.GetCustomerReportExcelAsync(request, await GetUser());
            return Ok(result);
        }

        /// <summary>
        /// Returns the report detail information by Id
        /// </summary>
        /// <param name="customerCode">Customer Code</param>
        /// /// <param name="campaignId">Campaign Id</param>
        [HttpGet]
        [Route("get-customer-report-detail/{customerCode}/{campaignId}")]
        public async Task<IActionResult> GetCustomerReportDetailAsync(string customerCode, int campaignId)
        {
            var result = await _reportService.GetCustomerReportDetailAsync(customerCode, campaignId, await GetUser());
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
