using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Public.Models.CampaignDocument;
using Bbt.Campaign.Services.Services.CampaignDocument;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CampaignDocumentController : BaseController<CampaignDocumentController>
    {
        private readonly ICampaignDocumentService _campaignDocumentService;

        public CampaignDocumentController(ICampaignDocumentService campaignDocumentService)
        {
            _campaignDocumentService = campaignDocumentService;
        }

        [HttpPost("add")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> Add([FromForm] AddCampaignDocumentRequest campaignDocument)
        {
            var createResult = await _campaignDocumentService.AddAsync(campaignDocument);
            return Ok(createResult);
        }

        [HttpGet("get-campaign-documents")]
        public async Task<IActionResult> GetDocumentsByCampaign(int campaignId)
        {
            var createResult = await _campaignDocumentService.GetDocumentsByCampaign(campaignId);
            return Ok(createResult);
        }

        [HttpGet("get-documents-by-type")]
        public async Task<IActionResult> GetDocumentsByType(int campaignId, int documentTypeId)
        {
            var createResult = await _campaignDocumentService.GetDocumentsByType(campaignId, documentTypeId);
            return Ok(createResult);
        }

        [HttpPost("add-html-contents")]
        [RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<IActionResult> AddHtmContents([FromForm] AddCampaignHtmlContentRequest campaignDocument)
        {
            var createResult = await _campaignDocumentService.AddHtmContents(campaignDocument);
            return Ok(createResult);
        }

        [HttpGet("get-campaign-htmls")]
        public async Task<IActionResult> GetHtmlContentsByCampaign(int campaignId)
        {
            var createResult = await _campaignDocumentService.GetHtmlContentsByCampaign(campaignId);
            return Ok(createResult);
        }

    }
}
