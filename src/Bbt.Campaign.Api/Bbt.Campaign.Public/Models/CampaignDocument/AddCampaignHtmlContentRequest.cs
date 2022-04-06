namespace Bbt.Campaign.Public.Models.CampaignDocument
{
    public class AddCampaignHtmlContentRequest
    {
        public int CampaignId { get; set; }
        public string SummaryTr { get; set; }
        public string SummaryEn { get; set; }
        public string ContentTr { get; set; }
        public string ContentEn { get; set; }
        public string DetailTr { get; set; }
        public string DetailEn { get; set; }
    }
}
