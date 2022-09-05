using Bbt.Campaign.Core.BaseEntities;
using Bbt.Campaign.Public.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignAchievementEntity : AuditableEntity
    {

        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }

        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; }
        public CurrencyEntity? Currency { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Rate { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MaxUtilization { get; set; }
        public AchievementType Type { get; set; }

        [ForeignKey("AchievementType")]
        public int AchievementTypeId { get; set; }
        public AchievementTypeEntity AchievementType { get; set; }

        [ForeignKey("ActionOption")]
        public int? ActionOptionId { get; set; }
        public ActionOptionEntity? ActionOption { get; set; }
        public string? DescriptionTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? TitleTr { get; set; }
        public string? TitleEn { get; set; }

        [MaxLength(7)]
        public string? XKAMPCode { get; set; }

        [MaxLength(100)]
        public string? Code { get; set; }
    }
}
