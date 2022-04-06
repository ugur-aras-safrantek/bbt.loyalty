using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Campaign
{
    public class CampaignListFilterResponse:PagingResponse
    {
        public CampaignListFilterResponse()
        {
            ResponseList = new List<CampaignListDto>();
        }
        public List<CampaignListDto> ResponseList { get; set; }
    }
}
