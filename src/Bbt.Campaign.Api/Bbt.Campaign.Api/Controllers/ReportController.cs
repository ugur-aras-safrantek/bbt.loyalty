using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Report;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Bbt.Campaign.Api.Controllers
{
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
        public async Task<IActionResult> FillCampaignFormAsync([FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.FillCampaignFormAsync(General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaignreport-by-filter")]
        public async Task<IActionResult> GetCampaignReportByFilterAsync(CampaignReportRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.GetCampaignReportByFilterAsync(request, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaign-report-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignReportRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.GetCampaignReportExcelAsync(request, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for customer report form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-customer-report-form")]
        public async Task<IActionResult> FillCustomerFormAsync([FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.FillCustomerFormAsync(General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter")]
        public async Task<IActionResult> GetCustomerReportByFilterAsync(CustomerReportRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.GetCustomerReportByFilterAsync(request, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }


        /// <summary>
        /// Returns the customer report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter-excel")]
        public async Task<IActionResult> GetCustomerReportExcelAsync(CustomerReportRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.GetCustomerReportExcelAsync(request, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }

        /// <summary>
        /// Returns the report detail information by Id
        /// </summary>
        /// <param name="id">Record Id of the line</param>
        [HttpGet]
        [Route("get-customer-report-detail/{id}")]
        public async Task<IActionResult> GetCustomerReportDetailAsync(int id, [FromHeader(Name = "userid")][Required] string userId)
        {
            var result = await _reportService.GetCustomerReportDetailAsync(id, General.GetUserIdFromHeader(Request));
            return Ok(result);
        }
    }
}
