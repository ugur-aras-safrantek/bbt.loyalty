using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Branch;
using Microsoft.AspNetCore.Http;

namespace Bbt.Campaign.Public.Models.CampaignRule
{
    public class AddCampaignRuleRequest
    {
        public int CampaignId { get; set; }
        public int JoinTypeId { get; set; }
        public bool IsSingleIdentity { get; set; }
        public string? Identity { get; set; }
        public string? File { get; set; }
        public List<int>? BusinessLines { get; set; }
        public List<string>? Branches { get; set; }
        public List<int>? CustomerTypes { get; set; }
        public int StartTermId { get; set; }
        public bool IsEmployeeIncluded { get; set; }
        public bool IsPrivateBanking { get; set; }
    }
}
