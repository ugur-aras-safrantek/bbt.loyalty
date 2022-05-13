

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportFormDto
    {
        public List<ParameterDto> CustomerTypeList { get; set; }
        public List<ParameterDto> CampaignStartTermList { get; set; }
        public List<ParameterDto> BusinessLineList { get; set; }
        public List<ParameterDto> AchievementTypes { get; set; }
    }
}
