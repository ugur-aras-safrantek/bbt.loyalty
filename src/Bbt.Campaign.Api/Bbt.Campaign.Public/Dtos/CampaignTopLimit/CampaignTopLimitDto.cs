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
        public string? MaxTopLimitAmount { get; set; }
        public string? MaxTopLimitRate { get; set; }
        public string? MaxTopLimitUtilization { get; set; }
        public TopLimitType Type { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDraft { get; set; }
        public int? RefId { get; set; }

    }
}
