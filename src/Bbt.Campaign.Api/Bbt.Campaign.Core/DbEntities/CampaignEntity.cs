using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignEntity : AuditableEntity
    {
        [MaxLength(500), Required]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Code { get; set; }

        [MaxLength(1000)]
        public string DescriptionTr { get; set; }

        [MaxLength(1000)]
        public string DescriptionEn { get; set; }

        [MaxLength(1000)]
        public string? TitleTr { get; set; } //başlık

        [MaxLength(1000)]
        public string? TitleEn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Order { get; set; }
        public int? MaxNumberOfUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public bool IsContract { get; set; }
        public int? ContractId { get; set; }

        [ForeignKey("Sector")]
        public int? SectorId { get; set; }
        public SectorEntity? Sector { get; set; }

        [ForeignKey("ViewOption")]
        public int? ViewOptionId { get; set; }
        public ViewOptionEntity? ViewOption { get; set; }

        [ForeignKey("ProgramType")]
        public int ProgramTypeId { get; set; }
        public ProgramTypeEntity ProgramType { get; set; }

        [ForeignKey("ParticipationType")]
        public int ParticipationTypeId { get; set; }
        public ParticipationTypeEntity ParticipationType { get; set; }
        public virtual CampaignDetailEntity CampaignDetail { get; set; }

        //Approve
        [MaxLength(100)]
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [ForeignKey("Statuses")]
        public int StatusId { get; set; }
        public StatusEntity Status { get; set; }

        public virtual CampaignRuleEntity CampaignRule { get; set; }
    }
}
