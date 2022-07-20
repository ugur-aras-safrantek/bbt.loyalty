using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.CampaignIdentity
{
    public class CampaignIdentityListFilterRequest : PagingRequest
    {
        public int? CampaignId { get; set; }
        public int? IdentitySubTypeId { get; set; }
        public string? Identities { get; set; }
    }
}
