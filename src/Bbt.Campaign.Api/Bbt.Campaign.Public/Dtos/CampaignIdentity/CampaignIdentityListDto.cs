﻿
namespace Bbt.Campaign.Public.Dtos.CampaignIdentity
{
    public class CampaignIdentityListDto
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int? IdentitySubTypeId { get; set; }
        public string? IdentitySubTypeName { get; set; }
        public string Identities { get; set; }
    }
}
