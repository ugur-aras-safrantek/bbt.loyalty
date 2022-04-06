using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.CampaignTopLimit
{
    public class CampaignTopLimitListFilterResponse:PagingResponse
    {
        public CampaignTopLimitListFilterResponse()
        {
            ResponseList = new List<CampaignTopLimitListDto>();
        }
        public List<CampaignTopLimitListDto> ResponseList { get; set; }
    }
}
