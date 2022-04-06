using Bbt.Campaign.Public.Dtos.CampaignTopLimit;


namespace Bbt.Campaign.Public.Dtos.Approval
{
    public  class TopLimitApproveFormDto
    {
        public bool isNewRecord { get; set; }
        public TopLimitDto TopLimitDraft { get; set; }
        public TopLimitDto? TopLimit { get; set; }

        public List<ParameterDto> CampaignList { get; set; }
        public List<ParameterDto> AchievementFrequencyList { get; set; }
        public List<ParameterDto> CurrencyList { get; set; }
    }
}
