using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Target
{
    public class TargetListFilterRequest : PagingRequest
    {
        public string? Name { get; set; }
        public int? Id { get; set; }
        public int? TargetViewTypeId { get; set; }
        public int? TargetSourceId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        //public bool? IsDraft { get; set; }

    }
}
