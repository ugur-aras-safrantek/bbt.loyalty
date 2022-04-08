using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Dtos.CampaignTopLimit
{
    public class TopLimitDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ParameterDto> Campaigns { get; set; }
        public int AchievementFrequencyId { get; set; }
        public ParameterDto AchievementFrequency { get; set; }
        public int? CurrencyId { get; set; }
        public ParameterDto Currency { get; set; }
        public decimal? MaxTopLimitAmount { get; set; }
        public decimal? MaxTopLimitRate { get; set; }
        public decimal? MaxTopLimitUtilization { get; set; }
        public string? MaxTopLimitAmountStr { get; set; }
        public string? MaxTopLimitRateStr { get; set; }
        public string? MaxTopLimitUtilizationStr { get; set; }
        public TopLimitType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDraft { get; set; }
        public int? RefId { get; set; }

    }
}
