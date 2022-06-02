using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Campaign
{
    public class Document
    {
        public string? Content { get; set; }
        public string? MimeType { get; set; }
        public bool? IsHtml { get; set; }
        public bool? IsPdf { get; set; }
        public string? CreatedByInstanceId { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? CreatedByIP { get; set; }
        public string? CreatedBehalfOf { get; set; }
        public string? Id { get; set; }
        public string? CreatedAt { get; set; }
    }
}
