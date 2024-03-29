﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("CampaignDetailListView")]
    public class CampaignDetailListEntity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string DescriptionTr { get; set; }
        public string DescriptionEn { get; set; }
        public string? TitleTr { get; set; } //başlık
        public string? TitleEn { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? Order { get; set; }
        public int? MaxNumberOfUser { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public bool IsContract { get; set; }
        public int? ContractId { get; set; }

        //join fields
        public int? SectorId { get; set; }
        public int? ViewOptionId { get; set; }
        public int? ProgramTypeId { get; set; }
        public int? ParticipationTypeId { get; set; }
        public int StatusId { get; set; }

        public string? SectorName { get; set; }
        public string? ViewOptionName { get; set; }
        public string? ProgramTypeName { get; set; }
        public string? ParticipationTypeName { get; set; }
        public string StatusName { get; set; }

        //Auditable entity fields
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }

        //detail fields
        public int CampaignDetailId { get; set; }
        public string? CampaignListImageUrl { get; set; }
        public string? CampaignDetailImageUrl { get; set; }
        public string? SummaryTr { get; set; }
        public string? SummaryEn { get; set; }
        public string? ContentTr { get; set; }
        public string? ContentEn { get; set; }
        public string? DetailTr { get; set; }
        public string? DetailEn { get; set; }
    }
}
