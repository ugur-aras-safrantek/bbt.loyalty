using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Models.CampaignDocument
{
    public class GetCampaignDocumentResponse
    {
        public List<DocumentModel> Documents { get; set; }
    }
    public class DocumentModel
    {
        public string DocumentName { get; set; }
        public DocumentTypePublicEnum DocumentType { get; set; }
        public string Data { get; set; }
        public string MimeType { get; set; }
    }
}
