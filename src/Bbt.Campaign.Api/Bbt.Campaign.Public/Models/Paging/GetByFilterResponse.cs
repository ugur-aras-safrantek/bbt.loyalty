using Bbt.Campaign.Public.Dtos.Paging;

namespace Bbt.Campaign.Public.Models.Paging
{
    public class GetByFilterResponse<T> where T : class
    {
        public GetByFilterResponse()
        {
            ResponseList = new List<T>(); 
        }

        public List<T> ResponseList { get; set; }
        public static PagingDto Paging { get; set; }
    }
}
