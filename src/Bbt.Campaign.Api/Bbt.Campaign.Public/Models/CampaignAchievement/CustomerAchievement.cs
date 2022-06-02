using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.CampaignAchievement
{
    public class CustomerAchievement
    {
        public bool IsAchieved { get; set; }
        public string AchievementTypeName { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public string? AmountStr { get; set; }
        public string? CurrencyCode { get; set; }
    }
}
