using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.Report;
using Bbt.Campaign.Services.Services.Report;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class ReportController : BaseController<ReportController>
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        [Route("get-campaign-report-form")]
        public async Task<IActionResult> FillCampaignFormAsync()
        {
            var result = await _reportService.FillCampaignFormAsync();
            return Ok(result);
        }

        [HttpPost]
        [Route("get-campaignreport-by-filter")]
        public async Task<IActionResult> GetCampaignByFilterAsync(CampaignReportListFilterRequest request)
        {
            var result = await _reportService.GetCampaignByFilterAsync(request);
            return Ok(result);
        }
    }
}
