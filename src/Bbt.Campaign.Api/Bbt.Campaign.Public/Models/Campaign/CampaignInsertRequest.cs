using Bbt.Campaign.Public.Dtos.CampaignDetail;

namespace Bbt.Campaign.Public.Models.Campaign
{
    public class CampaignInsertRequest
    {
        public string Name { get; set; }
        public string DescriptionTr { get; set; }
        public string DescriptionEn { get; set; }
        public string? TitleTr { get; set; }
        public string? TitleEn { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? Order { get; set; }
        public int? MaxNumberOfUser { get; set; }
        public int? SectorId { get; set; }
        public int? ViewOptionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsContract { get; set; }
        public bool IsBundle { get; set; }
        public int? ContractId { get; set; }
        public int ProgramTypeId { get; set; }

        public CampaignDetailDto  CampaignDetail { get; set; }
    }
}
