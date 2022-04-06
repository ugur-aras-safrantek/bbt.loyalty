using Bbt.Campaign.Public.Dtos.CampaignDetail;
using System.ComponentModel.DataAnnotations;

namespace Bbt.Campaign.Public.Dtos.CampaignDraft
{
    public class CampaignDraftDto
    {
        public int? CampaignId { get; set; }

        [MaxLength(500), Required]
        public string Name { get; set; }

        [MaxLength(100), Required]
        public string Code { get; set; }
        public string DescriptionDetail { get; set; }
        public string DescriptionDetailEnglish { get; set; }
        public string TitleDetail { get; set; }
        public string TitleDetailEnglish { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Order { get; set; }
        public int? MaxNumberOfUser { get; set; }

        public int SectorId { get; set; }
        public SectorDto Sector { get; set; }

        public int ViewOptionId { get; set; }
        public ViewOptionDto ViewOption { get; set; }

        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }

        public int Id { get; set; }
        public int? ContractId { get; set; }
        public int ProgramTypeId { get; set; }
        public CampaignDetailDto CampaignDetail { get; set; }
    }
}
