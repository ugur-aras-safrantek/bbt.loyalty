﻿using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Api.Extensions;
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
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

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
        public async Task<IActionResult> GetCampaignReportByFilterAsync(CampaignReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetCampaignReportByFilterAsync(request);
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
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetCampaignReportExcelAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for earning report form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-earning-report-form")]
        public async Task<IActionResult> FillEarningFormAsync()
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.FillEarningFormAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the earning report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-earning-report-by-filter")]
        public async Task<IActionResult> GetEarningReportByFilterAsync(CustomerReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetEarningReportByFilterAsync(request);
            return Ok(result);
        }


        /// <summary>
        /// Returns the earning report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-earning-report-by-filter-excel")]
        public async Task<IActionResult> GetEarningReportExcelAsync(CustomerReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetEarningReportExcelAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the customer report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter")]
        public async Task<IActionResult> GetCustomerReportByFilterAsync(CustomerCampaignReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetCustomerReportByFilterAsync(request);
            return Ok(result);
        }


        /// <summary>
        /// Returns the customer report excel file data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-customer-report-by-filter-excel")]
        public async Task<IActionResult> GetCustomerReportExcelAsync(CustomerCampaignReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetCustomerReportExcelAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the report detail information by Id
        /// </summary>
        /// <param name="customerCode">Customer Code</param>
        /// /// <param name="campaignCode">Campaign Code</param>
        [HttpGet]
        [Route("get-customer-report-detail/{customerCode}/{campaignCode}")]
        public async Task<IActionResult> GetCustomerReportDetailAsync(string customerCode, string campaignCode, [FromQuery] string term = null)
        {
            var result = await _reportService.GetCustomerReportDetailAsync(customerCode, campaignCode, term);
            return Ok(result);
        }

        /// <summary>
        /// Returns the form data for target report form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-target-report-form")]
        public async Task<IActionResult> FillTargetFormAsync()
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.FillTargetFormAsync();
            return Ok(result);
        }

        /// <summary>
        /// Returns the target report data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-targetreport-by-filter")]
        public async Task<IActionResult> GetTargetReportByFilterAsync(TargetReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetTargetReportByFilterAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Returns the target report excel data by selected filter options
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("get-target-report-by-filter-excel")]
        public async Task<IActionResult> GetTargetReportExcelAsync(TargetReportRequest request)
        {
            if (!(User.IsInRole("IsLoyaltyReader") || User.IsInRole("IsLoyaltyCreator")))
                throw new Exception(ControllerStatics.UnAuthorizedUserAlert);

            var result = await _reportService.GetTargetReportExcelAsync(request);
            return Ok(result);
        }
    }
}
