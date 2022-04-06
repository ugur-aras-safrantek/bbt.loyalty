using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Campaign_Document
{
    public class CampaignDocumentDto
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
       // public CampaignEntity Campaign { get; set; }

        //public DocumentTypeDbEnum DocumentType { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }
        public string DocumentName { get; set; }
    }
}
