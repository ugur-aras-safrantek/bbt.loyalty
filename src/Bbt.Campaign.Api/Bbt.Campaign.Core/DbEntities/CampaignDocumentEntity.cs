using Bbt.Campaign.Core.BaseEntities;
using Bbt.Campaign.Core.Enums;
using Bbt.Campaign.Shared.Extentions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignDocumentEntity : AuditableEntity
    {
        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }

        public DocumentTypeDbEnum DocumentType { get; set; }
        [DoNotLog]
        public byte[] Content { get; set; }

        public string MimeType { get; set; }
        public string DocumentName { get; set; }

        public string GetFileExtension()
        {
            return MimeType.ToFileExtension();
        }

        public CampaignDocumentEntity SetHtmlContent(string content)
        {
            return SetHtmlContent(System.Text.Encoding.UTF8.GetBytes(content));
        }

        public CampaignDocumentEntity SetHtmlContent(byte[] content)
        {
            Content = content;
            MimeType = MimeTypeExtensions.HtmlMimeType;
            return this;
        }

        public CampaignDocumentEntity SetPdfContent(string content)
        {
            return SetPdfContent(System.Text.Encoding.UTF8.GetBytes(content));
        }

        public CampaignDocumentEntity SetPdfContent(byte[] content)
        {
            Content = content;
            MimeType = MimeTypeExtensions.PdfMimeType;
            return this;
        }

        public bool IsHtml
        {
            get
            {
                return MimeType.IsHtml();
            }
        }

        public bool IsPdf
        {
            get
            {
                return MimeType.IsPdf();
            }
        }
    }
}
