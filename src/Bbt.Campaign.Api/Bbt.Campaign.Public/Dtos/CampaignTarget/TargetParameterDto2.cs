using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class TargetParameterDto2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int TargetViewTypeId { get; set; }
        public int TargetGroupId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public string? UsedAmountStr { get; set; }
        public string? usedAmountCurrencyCode { get; set; }
        public decimal? RemainAmount { get; set; }
        public string? RemainAmountStr { get; set; }
        public int Percent { get; set; }      
        public int UsedNumberOfTransaction { get; set; }
        public int? NumberOfTransaction { get; set; }
        public string? DescriptionTr { get; set; }

        public bool IsAchieved { get; set; }
    }
}
