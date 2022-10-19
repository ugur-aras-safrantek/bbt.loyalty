using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.CampaignAchievement
{
    public class EarningByCustomerAndCampaing
    {
        public string? EarningType { get; set; }
        public string? AchievementTitle { get; set; }
        public string? AchievementDescription { get; set; }
        public double Amount { get; set; }
        public string? Currency { get; set; }
        public string? Info { get; set; }
    }
}
