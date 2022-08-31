using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignAchievement
{
    public class CampaignAchievementFormDto
    {
        public int? CurrencyId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Rate { get; set; }
        public decimal? MaxAmount { get; set; }
        public decimal? MaxUtilization { get; set; }
        public int Type { get; set; }
        public int AchievementTypeId { get; set; }
        public int? ActionOptionId { get; set; }
        public string? DescriptionTr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? TitleTr { get; set; }
        public string? TitleEn { get; set; }
        public string? XKAMPCode { get; set; }
        public string? Code { get; set; }
    }
}
