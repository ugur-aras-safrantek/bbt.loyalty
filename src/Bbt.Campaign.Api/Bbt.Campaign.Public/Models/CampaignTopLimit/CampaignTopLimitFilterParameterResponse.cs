using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;

namespace Bbt.Campaign.Public.Models.CampaignTopLimit
{
    public class CampaignTopLimitFilterParameterResponse: CampaignTopLimitInsertFormDto
    {
        public List<ParameterDto> TypeList { get; set; }
    }
}
