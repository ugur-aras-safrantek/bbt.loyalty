using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Target
{
    public class TargetListFilterResponse : PagingResponse
    {
        public TargetListFilterResponse()
        {
            ResponseList = new List<TargetListDto>();
        }
        public List<TargetListDto> ResponseList { get; set; }

    }
}
