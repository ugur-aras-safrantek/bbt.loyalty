using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.CampaignIdentity
{
    public class UpdateCampaignIdentityRequest
    {
        public int CampaignId { get; set; }
        public int? IdentitySubTypeId { get; set; }
        public bool IsSingleIdentity { get; set; }
        public string? Identity { get; set; }
        public string? File { get; set; }
    }
}
