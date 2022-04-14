using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Campaign
{
    public class CampaignListFilterRequest : PagingRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? ContractId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBundle { get; set; }
        public int? ProgramTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsDraft { get; set; }

    }
}
