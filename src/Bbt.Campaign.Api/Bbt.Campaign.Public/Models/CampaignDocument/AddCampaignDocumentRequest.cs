using Bbt.Campaign.Public.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Bbt.Campaign.Public.Models.CampaignDocument
{
    public class AddCampaignDocumentRequest
    {
        [Required]
        public int CampaignId { get; set; }
        public IFormFile CampaignListImage { get; set; }
        public string CampaignListImageName { get; set; }
        public IFormFile CampaignDetailImage { get; set; }
        public string CampaignDetailImageName { get; set; }
    }
}
