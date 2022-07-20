
using Bbt.Campaign.Public.Dtos.CampaignIdentity;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.CampaignIdentity
{
    public class CampaignIdentityListFilterResponse : PagingResponse
    {
        public CampaignIdentityListFilterResponse()
        {
            CampaignIdentityList = new List<CampaignIdentityListDto>();
        }
        public List<CampaignIdentityListDto> CampaignIdentityList { get; set; }
    }
}
