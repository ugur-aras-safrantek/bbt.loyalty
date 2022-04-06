using Bbt.Campaign.Public.Dtos.CampaignDetail;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignDto
    {
        [MaxLength(500), Required]
        public string Name { get; set; }

        [MaxLength(100), Required]
        public string Code { get; set; }
        public string DescriptionTr { get; set; }
        public string DescriptionEn { get; set; }
        public string? TitleTr { get; set; }
        public string? TitleEn { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? Order { get; set; }
        public int? MaxNumberOfUser { get; set; }

        public int? SectorId { get; set; }
        public SectorDto? Sector { get; set; }

        public int? ViewOptionId { get; set; }
        public ViewOptionDto? ViewOption { get; set; }

        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public int Id { get; set; }
        public bool IsContract { get; set; }
        public int? ContractId { get; set; }
        public int ProgramTypeId { get; set; }

        //Approve
        public bool IsApproved { get; set; }
        public bool IsDraft { get; set; }
        public int? DraftId { get; set; }

        public CampaignDetailDto CampaignDetail { get; set; } 
    }
}
