namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignInsertFormDto
    {
        public List<ParameterDto> SectorList { get; set; }
        public List<ParameterDto> ViewOptionList { get; set; }
        public List<ParameterDto> ActionOptionList { get; set; }
        public List<ParameterDto> ProgramTypeList { get; set; }
    }
}
