using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignParameterFormDto
    {
        public List<ParameterDto> SectorList { get; set; }
        public List<ParameterDto> ViewOptionList { get; set; }
        public List<ParameterDto> ActionOptionList { get; set; }
        public List<ParameterDto> ProgramTypeList { get; set; }
        public List<ParameterDto> ActionOptions { get; set; }
        public List<ParameterDto> AchievementTypes { get; set; }
        public List<ParameterDto> CurrencyList { get; set; }
        public List<string> ChannelCodeList { get; set; }
        public List<ParameterDto> BusinessLineList { get; set; }
        public List<ParameterDto> JoinTypeList { get; set; }
        public List<ParameterDto> BranchList { get; set; }
        public List<ParameterDto> CustomerTypeList { get; set; }
        public List<ParameterDto> CampaignStartTermList { get; set; }
    }
}
