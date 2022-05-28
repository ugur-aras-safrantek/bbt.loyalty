using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CampaignReportRequest : PagingRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int? ContractId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBundle { get; set; }    
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool? IsApproved { get; set; }
        public int? SectorId { get; set; }
        public int? ViewOptionId { get; set; }        
        public int? ProgramTypeId { get; set; }
        public int? AchievementTypeId { get; set; }//kazanım tipi
        public int? JoinTypeId { get; set; } //rule - Dahil Olma Şekli
        public int? StatusId { get; set; }
    }
}
