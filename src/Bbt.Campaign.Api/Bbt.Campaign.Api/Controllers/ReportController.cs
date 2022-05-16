﻿using Bbt.Campaign.Api.Base;
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
        public async Task<IActionResult> FillCampaignFormAsync()
        {
            var result = await _reportService.FillCampaignFormAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaignreport-by-filter")]
        public async Task<IActionResult> GetCampaignByFilterAsync(CampaignReportListFilterRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            string userid = Request.Headers["userid"].ToString();

            var result = await _reportService.GetCampaignByFilterAsync(request, userid);
            return Ok(result);
        }

        /// <summary>
        /// Returns the campaign report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-campaignreport-by-filter-excel")]
        public async Task<IActionResult> GetByFilterExcel(CampaignReportListFilterRequest request, [FromHeader(Name = "userid")][Required] string userId)
        {
            var userid = Request.Headers["userid"].ToString();
            var result = await _reportService.GetCampaignReportExcelAsync(request, userid);
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
            var result = await _reportService.FillCustomerFormAsync();
            return Ok(result);
        }



        [HttpPost]
        public IActionResult Test([FromHeader(Name = "UserId")][Required] string requiredHeader)
        {
            return Ok();
        }
    }
}
