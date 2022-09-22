﻿using Bbt.Campaign.Public.Models.CampaignDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.File
{
    public class GetFileResponse
    {
        public DocumentModel Document { get; set; }
        public string? DocumentTextTr { get; set; }
        public string? DocumentTextEn { get; set; }
        public string? UnderlineTextTr { get; set; }
        public string? UnderlineTextEn { get; set; }
        public string? ButtonTextTr { get; set; }
        public string? ButtonTextEn { get; set; }
    }
}
