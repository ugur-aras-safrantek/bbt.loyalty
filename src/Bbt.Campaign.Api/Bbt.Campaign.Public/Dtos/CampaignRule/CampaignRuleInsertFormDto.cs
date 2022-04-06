namespace Bbt.Campaign.Public.Dtos.CampaignRule
{
    public class CampaignRuleInsertFormDto
    {
        public List<ParameterDto> BusinessLineList { get; set; }
        public List<ParameterDto> JoinTypeList { get; set; }
        public List<ParameterDto> BranchList { get; set; }
        public List<ParameterDto> CustomerTypeList { get; set; }
        public List<ParameterDto> CampaignStartTermList { get; set; }
    }
}
